using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBombDropper : NetworkBehaviour
{
    public PlayerEntity playerEntity;

    public int bombCount = 1;
    public int bombOnMap;
    public int bombMaxOnMap = 10;

    public int flamesPower = 1;
    public int flamePowerMax = 15;

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
        if (playerEntity.hubIdentity.isLocalPlayer && bombOnMap < bombCount)
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
    public void RpcDecrementBombOnMap()
    {
        if (bombOnMap > 0)
        {
            bombOnMap--;
        }
    }

    [ClientRpc]
    public void AddBombOnMap()
    {
        bombOnMap++;
    }

    [ClientRpc]
    public void RpcModifyBombCount(int amount)
    {
        bombCount += amount;

        if (bombCount <= 0)
            bombCount = 1;

        if (bombCount >= bombMaxOnMap)
            bombCount = bombMaxOnMap;
    }

    [ClientRpc]
    public void RpcModifyFlamesPower(int amount)
    {

        flamesPower += amount;

        if (flamesPower <= 0)
            flamesPower = 1;

        if (flamesPower >= flamePowerMax)
            flamesPower = flamePowerMax;

    }

    private void DropBomb()
    {
        Tile tile = LevelBuilder.grid.GetGridObject(transform.position);

        if (tile.type < ElementType.Block)
        {
            ItemBomb bomb = PoolingSystem.instance.GetPoolObject(ItemsType.BOMB) as ItemBomb;
            bomb.Teleport(LevelBuilder.grid.GetGridObjectWorldCenter(transform.position));
            bomb.bombDropper = this;
            bomb.flamesPower = flamesPower;

            AddBombOnMap();
        }
    }
}
