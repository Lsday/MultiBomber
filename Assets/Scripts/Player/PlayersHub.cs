using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is instanciated for each client
/// it is managing all the players physically playing through this client
/// </summary>
public class PlayersHub : NetworkBehaviour
{
    #region Static declarations
    public static List<PlayersHub> instancesList = new List<PlayersHub>();

    public static PlayersHub instance;
    #endregion

    #region Properties
    public GameObject playerPrefab;
    public SO_Int localPlayersCount;
    public SO_Int totalPlayersCount;

    // TODO : private variable
    public List<PlayerEntity> players = new List<PlayerEntity>();
    #endregion

    #region Init
    private void Start()
    {
        transform.position = Vector3.zero;

        if (isLocalPlayer)
        {
            // identify this script as the local instance of this class
            instance = this;

            // create a Player for every connected input device (keyboard, gamepad)
            CreatePlayers();
        }
    }
    #endregion

    #region Local functions
    void CreatePlayers()
    {
        //localPlayersCount is updated to reflect all the connected input devices and bots initialized on this client
        for (byte i = 0; i < localPlayersCount.value; i++)
        {
            AddPlayer(i); 
        }
    }

    public void AddPlayer(byte localPlayerIndex)
    {
        if (isLocalPlayer)
        {
            CmdSpawnPlayer(netIdentity, localPlayerIndex);
        }
    }

    private void OnDestroy()
    {
        instancesList.Remove(this);
    }
    #endregion

    #region Network functions


    // TODO : this function create an avatar for the main game
    // it should also initialize another class to manage the options for this player setup menu
    [Command]
    private void CmdSpawnPlayer(NetworkIdentity identity , byte localPlayerIndex)
    {
        // instantiate de Player prefab
        GameObject playerGameObject = Instantiate(playerPrefab, transform.position, Quaternion.identity);

        // Get the Player Entity of this instance
        PlayerEntity playerEntity = playerGameObject.GetComponent<PlayerEntity>();
        
        // 
        playerEntity.SetHubIdentity(identity);
        playerEntity.SetLocalPlayerIndex(localPlayerIndex);

        playerEntity.debugName = playerPrefab.name + " Hub:" + identity.netId + " Index:" + localPlayerIndex;

        players.Add(playerEntity);

        NetworkServer.Spawn(playerGameObject, gameObject);

        RpcUpdateTotalCount();
    }

    [ClientRpc]
    public void RpcUpdateTotalCount()
    {
        totalPlayersCount.value = PlayerEntity.GetInstancesCount();
    }
    #endregion

}
