using System.Collections;
using UnityEngine;
using Mirror;


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

    public void Init(NetworkIdentity playerIdentity)
    {
        this.playerIdentity = playerIdentity;
        timer = GetComponent<Timer>();
        SubscribeEvents();
        timer.Init();
    }

    private void SubscribeEvents()
    {
        timer.onTimerEnd += BombExplode;
    }

    private void BombExplode()
    {
        Debug.Log("BombExplode, " + "playerIdentity, " + playerIdentity + " isServer = " + isServer);
        NetworkServer.UnSpawn(gameObject);

    }

}
