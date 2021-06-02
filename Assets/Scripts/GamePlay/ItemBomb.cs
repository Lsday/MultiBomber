using System.Collections;
using UnityEngine;
using Mirror;
using System;


//spawn par le serveur 
// set la tile où la bombe est posée
// activer Timer
// peut se désactiver et ne pas péter automatiquement, mais peut péter si est touché par une autre explosion
//faire péter la bombe , la bombe a une puissance ( en cases )
//Calculer les 4 directions de la bombes, et regarder si l'explo est bloqué
// Si une bombe est touché , elle doit péter après
// si touche un joueur , il meurt
// les cases qui ont pété deviennent "chaudes" et restent mortelles
// Animation de flammes
// 

public class ItemBomb : ItemBase
{
    public NetworkIdentity playerIdentity;
    Timer timer;

    
    public BombBehaviour bombBehaviour;

    public void Init()
    {
        //this.playerIdentity = playerIdentity;
        timer = GetComponent<Timer>();
        SubscribeEvents();
        timer.Init();
        Debug.Log("Bomb Init");
    }

    private void SubscribeEvents()
    {
        Debug.Log("SubscribeEvents");
        timer.onTimerEnd += Triggerbomb;
    }

    private void Triggerbomb()
    {
        Debug.Log("BombExplode");

        if (isServer)
        {
            BombExplosion();
        }
        timer.onTimerEnd -= Triggerbomb;
    }

    private void BombExplosion()
    {

         bombBehaviour.PerformAction(gameObject);

        #region version 1
        //Vector3Int[] directions = { Vector3Int.forward, Vector3Int.left, Vector3Int.right, Vector3Int.back };
        //for (int i = 0; i < directions.Length; i++)
        //{
        //    int distance = 0;

        //    for (int j = 0; j < bombBehaviour.bombRadius; j++)
        //    {
        //        Tile tile = LevelBuilder.grid.GetGridObject(parentTile.x + directions[i].x, parentTile.y + directions[i].z);

        //        if (tile.item is IDestroyable)
        //        {
        //            distance++;
        //            ((IDestroyable)tile.item).Destroy();
        //            break;
        //        }

        //        if (tile.playerEntity != null)
        //        {
        //            distance++;
        //            ((IKillable)tile.playerEntity).Kill();
        //            break;
        //        }

        //        if (tile.type == ElementType.Wall)
        //            break;
        //        else
        //            distance++;

        //    }
        //} 
        #endregion


        #region Version2
        //////Tile[] tiles = new Tile[4];
        //////int[] distanceOfflames = {0,0,0,0};

        //////for (int i = 1; i <= bombeRadius; i++)
        //////{

        //////    tiles[0] = LevelBuilder.grid.GetGridObject(parentTile.x - i, parentTile.y); // LEFT 
        //////    tiles[1] = LevelBuilder.grid.GetGridObject(parentTile.x + i, parentTile.y); // RIGHT
        //////    tiles[2] = LevelBuilder.grid.GetGridObject(parentTile.x, parentTile.y - i); // BOT
        //////    tiles[3] = LevelBuilder.grid.GetGridObject(parentTile.x, parentTile.y + i); // TOP

        //////    for (int j = 0; j < tiles.Length; i++)
        //////    {
        //////        CheckNearbyTile(tiles[i]);
        //////    }
        //////} 



        //    if (tiles[0].item is IDestroyable || tiles[0].type is ElementType.Wall || tiles[0].playerEntity != null)
        //    {
        //        if (tiles[0].type is ElementType.Wall)
        //        {
        //            // DO STUFF
        //        }
        //        if (tiles[0].item is IDestroyable)
        //        {
        //            ((IDestroyable)tiles[0].item).Destroy();
        //        }
        //        if (tiles[0].playerEntity != null)
        //        {
        //            ((IKillable)tiles[0].playerEntity).Kill();
        //        }
        //        distanceOfflames[0] = Mathf.Abs(parentTile.x - tiles[0].x);
        //    }
        //    if (tiles[1].item is IDestroyable || tiles[1].type is ElementType.Wall || tiles[1].playerEntity != null)
        //    {
        //        if (tiles[1].type is ElementType.Wall)
        //        {
        //            // DO STUFF
        //        }
        //        if (tiles[1].item is IDestroyable)
        //        {
        //            ((IDestroyable)tiles[1].item).Destroy();
        //        }
        //        if (tiles[1].playerEntity != null)
        //        {
        //            ((IKillable)tiles[1].playerEntity).Kill();
        //        }
        //        distanceOfflames[1] = Mathf.Abs(parentTile.x - tiles[1].x);
        //    }
        //    if (tiles[2].item is IDestroyable || tiles[2].type is ElementType.Wall || tiles[2].playerEntity != null)
        //    {
        //        if (tiles[2].type is ElementType.Wall)
        //        {
        //            // DO STUFF
        //        }
        //        if (tiles[2].item is IDestroyable)
        //        {
        //            ((IDestroyable)tiles[2].item).Destroy();
        //        }
        //        if (tiles[2].playerEntity != null)
        //        {
        //            ((IKillable)tiles[2].playerEntity).Kill();
        //        }
        //        distanceOfflames[2] = Mathf.Abs(parentTile.y - tiles[2].y);
        //    }
        //    if (tiles[3].item is IDestroyable || tiles[3].type is ElementType.Wall || tiles[3].playerEntity != null)
        //    {
        //        if (tiles[3].type is ElementType.Wall)
        //        {
        //            // DO STUFF
        //        }
        //        if (tiles[3].item is IDestroyable)
        //        {
        //            ((IDestroyable)tiles[3].item).Destroy();
        //        }
        //        if (tiles[3].playerEntity != null)
        //        {
        //            ((IKillable)tiles[3].playerEntity).Kill();
        //        }
        //        distanceOfflames[3] = Mathf.Abs(parentTile.y - tiles[3].y);
        //    }
        //}
        #endregion

        Disable();
    }

    #region MyRegion
    //private static void CheckNearbyTile(Tile tile)
    //{
    //    if (tile.type is ElementType.Wall)
    //    {
    //        // DO STUFF
    //    }
    //    if (tile.item is IDestroyable)
    //    {
    //        ((IDestroyable)tile.item).Destroy();
    //    }
    //    if (tile.playerEntity != null)
    //    {
    //        ((IKillable)tile.playerEntity).Kill();
    //    }
    //} 
    #endregion
}
