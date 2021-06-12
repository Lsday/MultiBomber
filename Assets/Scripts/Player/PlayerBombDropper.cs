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

    [SyncVar] bool canDropbomb = true;

    public Filter currentFilter;

    [SyncVar] bool bombShit = false;

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
            playerEntity.controllerDevice.inputs.onDropBombAction -= PlaceBomb;
        
    }

    //private void Update()
    //{
    //    if (bombShit)
    //    {
    //        PlaceBomb();
    //    }
    //}

    public void PlaceBomb()
    {
        if (!canDropbomb) return;
        
        if (playerEntity.hubIdentity.isLocalPlayer && bombOnMap < bombCount)
        {
            if (isServer)
                DropBomb();
            
            else
                CmdDropBomb();
        }
    }

    [Command]
    private void CmdDropBomb() => DropBomb();
    
    [ClientRpc]
    public void RpcDecrementBombOnMap()
    {
        if (bombOnMap > 0)
            bombOnMap--;
    }

    [ClientRpc]
    public void AddBombOnMap() => bombOnMap++;
    
    [ClientRpc]
    public void RpcModifyBombCount(int amount) => bombCount = Mathf.Clamp(bombCount + amount, 1, bombMaxOnMap);

    [ClientRpc]
    public void RpcModifyFlamesPower(int amount) => flamesPower = Mathf.Clamp(flamesPower + amount, 1, flamePowerMax);
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

    public void ToggleCanDropBomb() => canDropbomb = !canDropbomb;

    public void ToggleBombShit() => bombShit = !bombShit;

     




}
