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
// les cases qui ont pété deviennent "chaudes" et restent mortelles pendant un laps de temps
// Animation de flammes
// 

public class ItemBomb : ItemBase, IDestroyable
{
    public NetworkIdentity playerIdentity;
    Timer timer;
    bool alreadyTriggered;

    public int bombPower = 1;


    public BombBehaviour bombBehaviour;
    public BombDropBehaviour dropBehaviour;

    public Direction explosionDirection;


    public void Init()
    {

        //Debug.Log("Bomb Init");
        
        //timer = GetComponent<Timer>();
        //timer.Init();
        //timer.onTimerEnd += Triggerbomb;
        //alreadySetToExplose = false;
    }

    protected override void OnDisable()
    {
        Debug.Log( "Bomb " + netId + " Disabled");

        if (timer != null)
            timer.onTimerEnd -= TriggerBomb;

        explosionDirection = Direction.None;
        alreadyTriggered = false;

        base.OnDisable();
    }

    protected override void OnEnable()
    {
        //Debug.Log("Bomb " + netId + " Enabled");

        base.OnEnable();

        if(timer == null) timer = GetComponent<Timer>();

        timer.Init();
        timer.onTimerEnd += TriggerBomb;

        alreadyTriggered = false;
    }

    private void TriggerBomb()
    {
        alreadyTriggered = true;

        if (isServer)
        {
            BombExplosion();
        }
    }

    private void BombExplosion()
    {

        bombBehaviour.PerformAction(this);
        dropBehaviour.PerformAction(this);

        Disable();
    }

    public void Destroy(float delay = 0f)
    {
        if (alreadyTriggered) return;
        alreadyTriggered = true;

        timer.DelayedStart(delay);
    }

   
}
   




