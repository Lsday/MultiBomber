using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombDropper : NetworkBehaviour
{
//    public NetworkIdentity playerIdentity;
//    public GameObject bombPrefab;
//    [SyncVar] public byte bombCounter = 1;


//    private void Start()
//    {
//        playerIdentity = GetComponent<NetworkIdentity>();
//    }
//    //private void Update()
//    //{
    //    if (isLocalPlayer)
    //    {
    //        if (Input.GetKeyDown(KeyCode.Space) && bombCounter > 0)
    //        {
    //            CmdDropBomb();
    //        }
    //    }
    //}

    //[Command]
    //private void CmdDropBomb()
    //{
    //    Vector3 spawnpos = transform.position + transform.forward;
    //    GameObject bomb = Instantiate(bombPrefab, spawnpos, Quaternion.identity);

    //    Bomb bombScript = bomb.GetComponent<Bomb>();
    //    bombScript.playerIdentity = this.playerIdentity;
    //    bombScript.Init(playerIdentity);

    //    NetworkServer.Spawn(bomb);
    //    RemoveBomb(1);

    //}

   
    //void AddBomb(byte count)
    //{
    //    bombCounter += count;
    //}

    
    //void RemoveBomb(byte count)
    //{
    //    bombCounter -= count;
    //}

}
