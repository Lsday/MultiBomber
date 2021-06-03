using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombDropper : NetworkBehaviour
{
    public PlayerEntity playerEntity;
    public GameObject bombPrefab;
   
    private void Start()
    {
        playerEntity = GetComponent<PlayerEntity>();
        PhysicalDevice.OnSpacePressed += PlaceBomb;
    }

    public void PlaceBomb()
    {
        if (playerEntity.hubIdentity.isLocalPlayer)
        {
            if (isServer)
            {
                DropBomb();
            }
            else
                CmdDropBomb();
        }
    }

    [Command]
    private void CmdDropBomb()
    {
        DropBomb();
    }


    private void DropBomb()
    {
        ItemBomb bomb = PoolingSystem.instance.GetPoolObject(ItemsType.BOMB) as ItemBomb;
        int x, y;
        LevelBuilder.grid.GetXY(transform.position, out x, out y);
        bomb.Teleport(LevelBuilder.grid.GetGridObjectWorldCenter(x, y));
        //bomb.Init();
    }


    //void AddBomb(byte count)
    //{
    //    bombCounter += count;
    //}


    //void RemoveBomb(byte count)
    //{
    //    bombCounter -= count;
    //}

}
