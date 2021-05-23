using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayersHub : NetworkBehaviour
{
    public GameObject playerPrefab;

    [SyncVar] byte playerIndex;

    public List<PlayerMovement> players = new List<PlayerMovement>();

    private void OnGUI()
    {
        if (isLocalPlayer)
        {
            if (GUI.Button(new Rect(300, 5, 200, 25), "Add Player"))
            {
                AddPlayer();
            }

            GUI.Label(new Rect(500, 5, 100, 25), playerIndex.ToString());
        }
    }

    void AddPlayer()
    {
        if (isLocalPlayer)
        {
            CmdSpawnPlayer(this.netIdentity);
        }

    }

    void Update()
    {
        if (!isLocalPlayer) return;
    }

    [Command]
    private void CmdSpawnPlayer(NetworkIdentity identity)
    {
        GameObject player = Instantiate(playerPrefab, transform.position, Quaternion.identity);

        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
        playerMovement.SetHubIdentity(identity);
        playerMovement.playerIndex = playerIndex;
        playerIndex++;

        NetworkServer.Spawn(player, gameObject);

        players.Add(playerMovement);

        Response();
    }


    [ClientRpc]
    private void Response()
    {
        Debug.Log("yo");
    }
}
