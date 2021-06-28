using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
public class PlayerEntity : NetworkBehaviour
{

    public Action OnPlayerDied;
    public Action<Vector3> OnPlayerSpawnPosition;

    #region Properties

    public static List<PlayerEntity> instancesList = new List<PlayerEntity>();
  
    [SyncVar] public string debugName; // TODO : remove this later
    [SyncVar] public NetworkIdentity hubIdentity;
    [SyncVar] public byte localPlayerIndex;
    [SyncVar] public Color defaultColor;

    public PlayerInputData currentInputData;

    /// <summary>
    /// The device (physical=keyboard,gamepad or virtual=bot) used to control this player
    /// </summary>
    public DeviceEntity controllerDevice;

    [Range(0.1f,0.5f)]
    public float deathThreshold = 0.3f;

    public PlayerMovement playerMovement {get; private set;}
    public PlayerBombDropper playerBombDropper{get; private set;}
    public PlayerBonusManager playerBonusManager{get; private set;}
    public PlayerDiseaseManager playerDiseaseManager { get; private set; }
    public PlayerAnimations playerAnimation { get; private set; }

   

    // TODO : private variables
    public Renderer avatarRenderer;
    public Material avatarMaterial;

    [SyncVar] public int deathCount;
    [SyncVar] public bool isDead;

    public Vector3 spawnPoint;

    /// <summary>
    /// How long the player is locked in place
    /// </summary>
    float lockTime = 0f;

    public bool IsLocked
    {
        get
        {
            return lockTime > 0f;
        }
    }

    #endregion

    #region Init
    private void Awake()
    {
        avatarRenderer = GetComponentInChildren<Renderer>();
        avatarMaterial = avatarRenderer.material;

        playerMovement = GetComponent<PlayerMovement>();

        playerBombDropper = GetComponent<PlayerBombDropper>();
        
        playerBonusManager = GetComponent<PlayerBonusManager>();

        playerDiseaseManager = GetComponent<PlayerDiseaseManager>();
        playerDiseaseManager.Init(this);


        playerAnimation = GetComponent<PlayerAnimations>();
        playerAnimation.OnPlayerDiedAnimEnded += KillPlayer;
    }

    public void ResetVariables()
    {
        lockTime = 0;
        isDead = false;
    }


    /// <summary>
    /// This function returns the number of all players (local+remote)
    /// </summary>
    /// <returns>int</returns>
    public static int GetInstancesCount()
    {
        return instancesList.Count;
    }
    public static void SortInstances()
    {
        // TODO : add a "creation timestamp" to sort the players with it, instead of hubidentity+localindex
        instancesList.Sort((x, y) => {
            var ret = x.hubIdentity.netId.CompareTo(y.hubIdentity.netId);
            if (ret == 0) ret = x.localPlayerIndex.CompareTo(y.localPlayerIndex);
            return ret;
        });
    }

    public void SetSpawnPosition(Vector3 position)
    {
        spawnPoint = position;
        OnPlayerSpawnPosition?.Invoke(position);
    }

    public static PlayerEntity GetMajorPlayer(int x, int y)
    {
        float maxOccupation = 0;
        PlayerEntity player = null;
        for (int i = 0; i < instancesList.Count; i++)
        {
            float o = instancesList[i].GetTileOccupation(x, y);
            if ( o > maxOccupation && !instancesList[i].isDead)
            {
                maxOccupation = o;
                player = instancesList[i];
            }
        }

        return player;
    }
    /// <summary>
    /// Set the pointer to the NetworkIndentity of the PlayerHub which created this player.
    /// </summary>
    /// <param name="identity"></param>
    public void SetHubIdentity(NetworkIdentity identity)
    {
        hubIdentity = identity;
    }

    /// <summary>
    /// Index of this player on its local machine
    /// </summary>
    /// <param name="index"></param>
    public void SetLocalPlayerIndex(byte index)
    {
        localPlayerIndex = index;
    }

    private void OnEnable()
    {
        // Add this Player to the local instances list, to keep track of all the available players
        instancesList.Add(this);

        ResetVariables();

        // Sort the instances list, to keep the same order on all clients and the server
        if (hubIdentity) SortInstances();

        if (spawnPoint!=null)
        {
            OnPlayerSpawnPosition?.Invoke(spawnPoint);
        }
    }

    private void OnDisable()
    {
        instancesList.Remove(this);
        if (hubIdentity) SortInstances();
    }

    #endregion

    #region Local functions
    private void Start()
    {

        if (hubIdentity.isLocalPlayer)
        {
            if (controllerDevice == null)
            {
                controllerDevice = DeviceEntity.instancesList[localPlayerIndex];
            }
            SortInstances();
        }

        if (isServer)
        {
            defaultColor = UnityEngine.Random.ColorHSV(0,1,0,1,0.7f,1f);
            RpcSetColor(defaultColor);
        }

        ApplyColor();
    }

    public float GetTileOccupation(int x,int y)
    {
        if (Mathf.Abs(playerMovement.currentTileX - x) > 2 || Mathf.Abs(playerMovement.currentTileY - y) > 2)
            return 0f;

        for(int i = 0; i < playerMovement.tiles.Length; i++)
        {
            if(playerMovement.tiles[i].tile != null)
            {
                if(playerMovement.tiles[i].tile.x == x && playerMovement.tiles[i].tile.y == y)
                {
                    return playerMovement.tiles[i].occupation;
                }
            }
        }

        return 0;
    }

    void ApplyColor()
    {
        avatarMaterial.color = defaultColor;
    }
    #endregion

    
    private void Update()
    {
        if (!isServer || isDead) return;

        if(lockTime > 0)
        {
            lockTime -= Time.deltaTime;
        }

        if (CheckDeath())
        {
            isDead = true;

            // TODO : créer une classe de scoring
            deathCount++;

            playerBonusManager.GiveAllBonusBack();
            
            RPCPlayerDeath();
            
        }
    }

    public void SetLockTime(float duration)
    {
        lockTime = duration;
    }

    void KillPlayer()
    {
        gameObject.SetActive(false);
    }

    #region Network functions

    [ClientRpc]
    void RPCPlayerDeath()
    {
        OnPlayerDied?.Invoke();
        
    }


    [ClientRpc]
    void RpcSetColor(Color newColor)
    {
        defaultColor = newColor;
        ApplyColor();
    }

    void OnDrawGizmos()
    {
        if (CheckDeath())
        {
            Gizmos.color = Color.red;
            Gizmos.DrawCube(transform.position, Vector3.one);
        }
    }

    

    private bool CheckDeath()
    {
        for (int i = 0; i < playerMovement.tiles.Length; i++)
        {
            if (playerMovement.tiles[i].occupation > deathThreshold)
            {
                if(playerMovement.tiles[i].tile.temperature > 0)
                {
                    return true;
                }
            }
        }

        return false;
    }

    #endregion
}
