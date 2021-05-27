using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class PlayerEntity : NetworkBehaviour
{
    #region Static declarations
    public static List<PlayerEntity> instancesList = new List<PlayerEntity>();
    public static void SortInstances()
    {
        // TODO : add a "creation timestamp" to sort the players with it, instead of hubidentity+localindex
        instancesList.Sort((x, y) => {
            var ret = x.hubIdentity.netId.CompareTo(y.hubIdentity.netId);
            if (ret == 0) ret = x.localPlayerIndex.CompareTo(y.localPlayerIndex);
            return ret;
        });
    }

    /// <summary>
    /// This function returns the number of all players (local+remote)
    /// </summary>
    /// <returns></returns>
    public static int GetInstancesCount()
    {
        return instancesList.Count;
    }
    #endregion

    #region Properties
    [SyncVar] public string debugName; // TODO : remove this later
    [SyncVar] public NetworkIdentity hubIdentity;
    [SyncVar] public byte localPlayerIndex;
    [SyncVar] public Color defaultColor;

    /// <summary>
    /// The device (physical=keyboard,gamepad or virtual=bot) used to control this player
    /// </summary>
    public DeviceEntity controllerDevice;

    // TODO : private variables
    public Animator animator;
    public Renderer avatarRenderer;
    public Material avatarMaterial;
    #endregion

    #region Init
    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        avatarRenderer = GetComponentInChildren<Renderer>();
        avatarMaterial = avatarRenderer.material;
    }

    /// <summary>
    /// Pointer to the NetworkIndentity of the PlayerHub which created this player.
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

        // Sort the instances list, to keep the same order on all clients and the server
        if (hubIdentity) SortInstances();
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
            defaultColor = Random.ColorHSV();
            RpcSetColor(defaultColor);
        }

        ApplyColor();
    }

    void ApplyColor()
    {
        avatarMaterial.color = defaultColor;
    }
    #endregion


    #region Network functions
    [ClientRpc]
    void RpcSetColor(Color newColor)
    {
        defaultColor = newColor;
        ApplyColor();
    }
    #endregion
}
