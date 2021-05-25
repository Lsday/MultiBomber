using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayersHub : NetworkBehaviour
{
    public static List<PlayersHub> instancesList = new List<PlayersHub>();

    public static PlayersHub instance;

    public GameObject playerPrefab;
    public SO_Int localPlayersCount;
    public SO_Int totalPlayersCount;

    public List<PlayerEntity> players = new List<PlayerEntity>();

    private void Start()
    {
        transform.position = Vector3.zero;

        if (isLocalPlayer)
        {
            instance = this;
            CreatePlayers();
        }
    }
    
    void CreatePlayers()
    {
        for(byte i = 0; i < localPlayersCount.value; i++)
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

    [Command]
    private void CmdSpawnPlayer(NetworkIdentity identity , byte localPlayerIndex)
    {
        GameObject playerGameObject = Instantiate(playerPrefab, transform.position, Quaternion.identity);

        PlayerEntity playerEntity = playerGameObject.GetComponent<PlayerEntity>();
        playerEntity.transform.parent = transform;

        playerEntity.SetHubIdentity(identity);
        playerEntity.SetLocalPlayerIndex(localPlayerIndex);

        playerEntity.debugName = playerPrefab.name + " Hub:" + identity.netId + " Index:" + localPlayerIndex;

        players.Add(playerEntity);

        NetworkServer.Spawn(playerGameObject, gameObject);

        UpdateTotalCount();
    }

    [ClientRpc]
    public void UpdateTotalCount()
    {
        totalPlayersCount.value = PlayerEntity.GetInstancesCount();
    }
}
