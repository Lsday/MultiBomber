using UnityEngine;


public class ItemBomb : ItemBase, IDestroyable
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

    protected override void OnDisable()
    {
        //Debug.Log("BOMB OnDisable");
        explosionDirection = Direction.None;
        alreadyTriggered = false;

        base.OnDisable();
    }

    protected override void OnEnable()
    {
        //Debug.Log("BOMB OnEnable");
        base.OnEnable();

        timer.StartTimer();

        alreadyTriggered = false;
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

        bombBehaviour.PerformAction(this); // Calcul des directions que peuvent emprunter les flammes et stocke l'information dans "this"
        flamesBehaviour.PerformAction(this); // Spawn les flammes

        bombDropper.RpcDecrementBombOnMap();

        Disable();
    }

}
   




