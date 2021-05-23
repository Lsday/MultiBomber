using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayersHub : NetworkBehaviour
{
    public NetworkIdentity hubIdentity;
    public byte localPlayersCount = 3;
    public GameObject playerPrefab;

    [SyncVar] byte nextPlayerIndex;

    List<PlayerMovement> players = new List<PlayerMovement>();

    void Start()
    {
        hubIdentity = GetComponent<NetworkIdentity>();

        if (isLocalPlayer)
        {
            for (byte i = 0; i < localPlayersCount; i++)
            {
                CmdSpawnPlayer(nextPlayerIndex,i);
                nextPlayerIndex++;
            }
        }

    }

    public void LinkPlayer(PlayerMovement playerMovement)
    {
        players.Add(playerMovement);
    }

    void Update()
    {
        if (!isLocalPlayer) return;

        foreach (PlayerMovement p in players)
        {
            if(Input.GetKey(p.myKey))
            {
                p.SetMovement(new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")));
            }
        }

    }

    [Command]
    private void CmdSpawnPlayer(byte playerIndex,byte localIndex)
    {
        Vector3 spawnpos = transform.position;
        GameObject player = Instantiate(playerPrefab, spawnpos, Quaternion.identity);

        PlayerMovement playerScript = player.GetComponent<PlayerMovement>();

        KeyCode[] keys = { KeyCode.RightShift , KeyCode.LeftShift , KeyCode.LeftControl, KeyCode.RightControl };

        playerScript.Init(hubIdentity , playerIndex, keys[localIndex]);

        NetworkServer.Spawn(player);
    }
}
