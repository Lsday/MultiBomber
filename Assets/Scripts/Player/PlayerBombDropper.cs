using UnityEngine;
using Mirror;
using System;

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


    //[SyncVar] bool bombShit = false;

    private void Start()
    {
        playerEntity = GetComponent<PlayerEntity>();
        playerEntity.OnPlayerDied += ResetVariables;

        LinkInputActions();

    }

    public void ResetVariables()
    {
        flamesPower = 1;
        bombOnMap = 0;
        bombCount = 1;
        canDropbomb = true;
    }

    private void OnEnable()
    {
        LinkInputActions();
    }

    private void OnDisable()
    {
        if (playerEntity && playerEntity.hubIdentity.isLocalPlayer)
            playerEntity.controllerDevice.inputs.onDropBombAction -= PlaceBomb;

    }

    void LinkInputActions()
    {
        if (playerEntity && playerEntity.hubIdentity.isLocalPlayer)
        {
            if (playerEntity.controllerDevice.inputs.onDropBombAction != null)
            {
                playerEntity.controllerDevice.inputs.onDropBombAction -= PlaceBomb;
            }

            playerEntity.controllerDevice.inputs.onDropBombAction += PlaceBomb;
        }
    }



    public void SubscribeBombShitEvent()
    {
        Debug.Log("SubscribeBombShitEvent");
        if (isServer)
        {
            RpcSubscribeBombShitEvent();
        }
        else
        {
            playerEntity.playerMovement.OnTileEntered += PlaceBomb;
        }
    }

    [ClientRpc]
    public void RpcSubscribeBombShitEvent()
    {
        Debug.Log("RpcSubscribeBombShitEvent");
        playerEntity.playerMovement.OnTileEntered += PlaceBomb;
    }


    public void UnSubscribeBombShitEvent()
    {
        Debug.Log("UnSubscribeBombShitEvent");
        if (isServer)
        {
            RpcUnSubscribeBombShitEvent();
        }
        else
        {
            playerEntity.playerMovement.OnTileEntered -= PlaceBomb;
        }
    }

    [ClientRpc]
    public void RpcUnSubscribeBombShitEvent()
    {
        playerEntity.playerMovement.OnTileEntered -= PlaceBomb;
    }

    public void PlaceBomb()
    {
        if (!canDropbomb) return;
        if (playerEntity.isDead) return;


        if (playerEntity.hubIdentity.isLocalPlayer && bombOnMap < bombCount)
        {
            if (isServer)
                DropBomb();

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
            bombOnMap--;
    }

    [ClientRpc]
    public void AddBombOnMap()
    {
        bombOnMap++;
    }

    [ClientRpc]
    public void RpcModifyBombCount(int amount)
    {
        bombCount = Mathf.Clamp(bombCount + amount, 1, bombMaxOnMap);
    }

    [ClientRpc]
    public void RpcModifyFlamesPower(int amount)
    {
        flamesPower = Mathf.Clamp(flamesPower + amount, 1, flamePowerMax);
    } 
    private void DropBomb()
    {
        Tile tile = LevelBuilder.grid.GetGridObject(transform.position);

        if (tile.type < ElementType.Block)
        {
            ItemBomb bomb = PoolingSystem.instance.GetPoolObject(ItemsType.BOMB, LevelBuilder.grid.GetGridObjectWorldCenter(transform.position)) as ItemBomb;
            bomb.bombDropper = this;
            bomb.flamesPower = flamesPower;
            AddBombOnMap();
        }
    }

    public void ToggleCanDropBomb()
    {
        canDropbomb = !canDropbomb;
    } 


}
