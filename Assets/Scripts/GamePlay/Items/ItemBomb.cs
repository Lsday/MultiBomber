public class ItemBomb : ItemBase, IDestroyable
{
    public PlayerBombDropper bombDropper;
    Timer timer;
    bool alreadyTriggered;

    public int flamesPower = 1;
   


    public BombBehaviour bombBehaviour;
    public FlamesDropBehaviour flamesBehaviour;

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
        //Debug.Log( "Bomb " + netId + " Disabled");

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
        bombBehaviour.PerformAction(this); // Calcul des directions que peuvent emprunter les flammes et stocke l'information dans "this"
        flamesBehaviour.PerformAction(this); // Spawn les flammes

        bombDropper.RpcDecrementBombOnMap();

        Disable();
    }

    public void Destroy()
    {
        if (alreadyTriggered) return;
        alreadyTriggered = true;

        timer.EndTimerEarly();
    }

    public void InitDestroy(float delay = 0f, float fireEndDelay = 0f)
    {
        if (alreadyTriggered) return;
        alreadyTriggered = true;

        timer.DelayedStart(delay);
    }
}
   




