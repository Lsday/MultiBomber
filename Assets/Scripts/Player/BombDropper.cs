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
        LinkInputActions();
    }

    private void OnEnable()
    {
        LinkInputActions();
    }

    void LinkInputActions()
    {
        if (playerEntity && playerEntity.hubIdentity.isLocalPlayer)
        {
            playerEntity.controllerDevice.inputs.onDropBombAction -= PlaceBomb;
            playerEntity.controllerDevice.inputs.onDropBombAction += PlaceBomb;
        }
    }

    private void OnDisable()
    {
        if (playerEntity && playerEntity.hubIdentity.isLocalPlayer)
        {
            playerEntity.controllerDevice.inputs.onDropBombAction -= PlaceBomb;
        }
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
        Tile tile = LevelBuilder.grid.GetGridObject(transform.position);

        if(tile.type < ElementType.Block)
        {
            ItemBomb bomb = PoolingSystem.instance.GetPoolObject(ItemsType.BOMB) as ItemBomb;

            bomb.Teleport(LevelBuilder.grid.GetGridObjectWorldCenter(transform.position));
        }
        
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
