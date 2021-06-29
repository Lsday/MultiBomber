using UnityEngine;
using Mirror;

public class ItemBomb : ItemKickable, IDestroyable
{
    public PlayerBombDropper bombDropper;
    Timer timer;
    bool alreadyTriggered;

    public int flamesPower = 1;
   
    public BombBehaviour bombBehaviour;
    public FlamesDropBehaviour flamesBehaviour;

    public Direction explosionDirection;

    public override void Awake()
    {
        //Debug.Log("BOMB Awake");

        base.Awake();

        if (timer == null)
        {
            timer = GetComponent<Timer>();
            timer.onTimerEnd += TriggerBomb;
        }
    }

    public override void OnDisable()
    {

        explosionDirection = Direction.None;
        alreadyTriggered = false;

    }

    protected void OnEnable()
    {

        timer.StartTimer();

        alreadyTriggered = false;
    }

    [ClientRpc]
    public void RpcInitDestroy(float delay, float endDelay)
    {
        InitDestroy(delay, endDelay);
    }

    // fire end delay = how many time is left before the end of this explosion
    public void InitDestroy(float delay = 0f, float fireEndDelay = 0f)
    {
        
        //Debug.Log("InitDestroy");
        if (alreadyTriggered) return;
        alreadyTriggered = true;

        timer.StartTimer(delay);
    }

    public void Destroy()
    {
        //Debug.Log("Destroy");
        if (alreadyTriggered) return;
        alreadyTriggered = true;

        timer.EndTimerEarly();
    }

    private void TriggerBomb()
    {
        //Debug.Log("TriggerBomb");
        alreadyTriggered = true;

        if (isServer)
        {
            BombExplosion();
        }

    }

    private void BombExplosion()
    {
        //Debug.Log("BombExplosion");

        SetRoundPosition();
        
        bombBehaviour.PerformAction(this); // Calcul des directions que peuvent emprunter les flammes et stocke l'information dans "this"
        flamesBehaviour.PerformAction(this); // Spawn les flammes

        bombDropper.RpcDecrementBombOnMap();

        Disable();
    }

}
   




