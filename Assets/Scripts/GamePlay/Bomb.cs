using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : NetworkBehaviour
{
    public NetworkIdentity playerIdentity;
    Timer timer;

    public void Init(NetworkIdentity playerIdentity)
    {
        this.playerIdentity = playerIdentity;
        timer = GetComponent<Timer>();
        SubscribeEvents();
        timer.Init();
    }

    private void SubscribeEvents()
    {
        timer.onTimerEnd += BombExplode;
    }

    private void BombExplode()
    {
        Debug.Log("BombExplode, " + "playerIdentity, " + playerIdentity + " isServer = " + isServer);
        NetworkServer.UnSpawn(gameObject);

    }
}
