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

    public int bombeRadius;

    ScriptableAction bombBehaviour;

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
        // Vector3Int[] directions = { Vector3Int.forward, Vector3Int.left, Vector3Int.right, Vector3Int.back };
        //for (int i = 0; i < directions.Length; i++)
        //{
        //    int power = 0;

        //    for (int j = 0; j < bombeRadius; j++)
        //    {

        //        if (LevelBuilder.grid.GetGridObject(parentTile.x + directions[i].x, parentTile.y + directions[i].z).item is IKillable)
        //        {
        //            power++;
        //            break;
        //        }

        //        if (LevelBuilder.grid.GetGridObject(parentTile.x + directions[i].x, parentTile.y + directions[i].z).type == ElementType.Wall)
        //        {
        //            break;
        //        }
        //        else
        //        {
        //            power++;
        //        }
        //    }
        //}



        for (int i = 1; i <= bombeRadius; i++)
        {

            if (LevelBuilder.grid.GetGridObject(parentTile.x - i, parentTile.y) is IKillable)
            {

            }

            if (LevelBuilder.grid.GetGridObject(parentTile.x + i, parentTile.y) is IKillable)
            {

            }

            if (LevelBuilder.grid.GetGridObject(parentTile.x, parentTile.y - i) is IKillable)
            {

            }

            if (LevelBuilder.grid.GetGridObject(parentTile.x, parentTile.y + i) is IKillable)
            {

            }
        }


        



        Disable();
    }
}
