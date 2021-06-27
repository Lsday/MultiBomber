using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class LootManager : NetworkBehaviour
{
    [Range(0.01f,0.5f)]
    public float lootThreshold = 0.1f;
    void Update()
    {
        if (isServer)
        {
            for (int i = 0; i < PlayerEntity.instancesList.Count; i++)
            {
                PlayerEntity player = PlayerEntity.instancesList[i];

                if (player.isDead) continue;


                PlayerMovement.TileOccupation[] tiles = player.playerMovement.tiles;

                for (int j = 0; j < tiles.Length; j++)
                {
                    if (tiles[j].occupation > lootThreshold)
                    {
                        Tile currentTile = tiles[j].tile;

                        if (currentTile.type == ElementType.Item && currentTile.item != null)
                        {
                            PlayerEntity majorPlayer = PlayerEntity.GetMajorPlayer(currentTile.x, currentTile.y);
                            if(majorPlayer != null) ((ILootable)currentTile.item).Loot(majorPlayer);
                        }
                    }
                }
            }
        }
    }
}
