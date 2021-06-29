using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisions : MonoBehaviour
{
    PlayerMovement playerMovement;
    PlayerEntity playerEntity;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();

        playerMovement.OnPlayerMoved += DetectCollisions;

        playerEntity = GetComponent<PlayerEntity>();
    }

    private void DetectCollisions(int h,int v)
    {
        int x = playerMovement.currentTileX + h;
        int y = playerMovement.currentTileY + v;

        Tile tile = LevelBuilder.grid.GetGridObject(x, y);

        if(tile.item != null && tile.type == ElementType.Bomb)
        {

            ItemKickable item = tile.item as ItemKickable;
            int kickPower = 3;
            item.Kick(playerEntity, kickPower, h,v);
        }
    }
}
