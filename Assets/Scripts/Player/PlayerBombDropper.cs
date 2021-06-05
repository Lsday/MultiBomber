using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBombDropper : NetworkBehaviour
{
    public PlayerEntity playerEntity;

    public int bombMax = 1;
    public int bombCounter;
   
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
        if (playerEntity.hubIdentity.isLocalPlayer && bombCounter < bombMax)
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

    [ClientRpc]
    public void RpcDecrementBombCounter()
    {
        if (bombCounter > 0)
        {
            bombCounter--;
        }
    }

    [ClientRpc]
    public void RpcIncrementBombCounter()
    {
       
            bombCounter++;
        
    }

    [ClientRpc]
    public void RpcDecrementBombMax()
    {
        if (bombMax > 1)
        {
            bombMax--;
        }
    }

    [ClientRpc]
    public void RpcIncrementBombMax()
    {
        if (bombMax < 10)
        {
            bombMax++;
        }
       
    }

    private void DropBomb()
    {
        Tile tile = LevelBuilder.grid.GetGridObject(transform.position);

        if(tile.type < ElementType.Block)
        {
            ItemBomb bomb = PoolingSystem.instance.GetPoolObject(ItemsType.BOMB) as ItemBomb;

            bomb.Teleport(LevelBuilder.grid.GetGridObjectWorldCenter(transform.position));
            bomb.bombDropper = this;

            RpcIncrementBombCounter();
        }
    }






    void AddBomb(byte count)
    {
        bombCounter += count;
    }


    void RemoveBomb(byte count)
    {
        bombCounter -= count;
    }

}
