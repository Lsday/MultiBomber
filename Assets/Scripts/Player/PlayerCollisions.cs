using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class PlayerCollisions : NetworkBehaviour
{
    PlayerMovement playerMovement;
    PlayerEntity playerEntity;

    public int bombKickPower = 0;
    public int boxKickPower = 0;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();

        playerMovement.OnPlayerMoved += DetectCollisions;
        
        playerEntity = GetComponent<PlayerEntity>();
    }

    private void DetectCollisions(int h,int v)
    {

        if(bombKickPower > 0 || boxKickPower > 0)
        {
            int x = playerMovement.currentTileX + h;
            int y = playerMovement.currentTileY + v;

            Tile tile = LevelBuilder.grid.GetGridObject(x, y);

            if (tile.item != null &&( (tile.type == ElementType.Bomb && bombKickPower > 0) || (tile.type == ElementType.Box && boxKickPower > 0)))
            {
                Vector3 distance = tile.worldPosition - transform.position;
                if ((h != 0 && Mathf.Abs(distance.x) <= 1f) || (v != 0 && Mathf.Abs(distance.z) <= 1f))
                {
                    ItemKickable item = tile.item as ItemKickable;
                    item.Kick(playerEntity, tile.type == ElementType.Bomb ? bombKickPower: boxKickPower, h, v);
                }

            }
        }
        
    }

    [ClientRpc]
    public void RpcModifyBombKickPower(int amount)
    {
        bombKickPower += amount;
    }

    [ClientRpc]
    public void RpcModifyBoxKickPower(int amount)
    {
        boxKickPower += amount;
    }
}
