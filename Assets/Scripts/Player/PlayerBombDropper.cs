using Mirror;
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


    //[SyncVar] bool bombShit = false;

    private void Start()
    {
        playerEntity = GetComponent<PlayerEntity>();
        LinkInputActions();

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
        Debug.Log("RpcUnSubscribeBombShitEvent");
        playerEntity.playerMovement.OnTileEntered -= PlaceBomb;
    }

    public void PlaceBomb()
    {
        Debug.Log("PlaceBomb");
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
    private void CmdDropBomb()
    {
        Debug.Log("CmdDropBomb");
        DropBomb();
      
    }

    [ClientRpc]
    public void RpcDecrementBombOnMap()
    {
        Debug.Log("RpcDecrementBombOnMap");
        if (bombOnMap > 0)
            bombOnMap--;
    }

    [ClientRpc]
    public void AddBombOnMap()
    {
        Debug.Log("AddBombOnMap");
        bombOnMap++;
    }

    [ClientRpc]
    public void RpcModifyBombCount(int amount)
    {
        Debug.Log("RpcModifyBombCount");
        bombCount = Mathf.Clamp(bombCount + amount, 1, bombMaxOnMap);
    }

    [ClientRpc]
    public void RpcModifyFlamesPower(int amount)
    {
        Debug.Log("RpcModifyFlamesPower");
        flamesPower = Mathf.Clamp(flamesPower + amount, 1, flamePowerMax);
    } 
    private void DropBomb()
    {
        Debug.Log("DropBomb");
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

    public void ToggleCanDropBomb()
    {
        Debug.Log("ToggleCanDropBomb");
        canDropbomb = !canDropbomb;
    } 


}
