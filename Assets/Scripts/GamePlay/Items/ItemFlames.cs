using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Mirror;

public class ItemFlames : ItemBase
{

    public Vector3 direction;

    private float startPower = 1f;
    private float endPower = 1f;
    private float currentPower = 1f;

    // remplissage de la tile en bout d'explosition
    // pour que les particules occupent bien l'espace de cette dernière case
    [Range(0f, 0.5f)]
    public float cellFilling = 0.4f;

    [Range(1, 10)]
    public int instantFire = 4;

    // durée minimale de visibilité de la flamme
    [Range(0.5f, 2f)]
    public float duration = 1f;


    // durée de visibilité supplémentaire pour chaque niveau de flamme 
    [Range(0f, 0.3f)]
    public float expansionCellTime = 0.16f;

    // durée de l'agrandissement du diamètre de la flamme au moment de son apparition
    [Range(0f, 0.3f)]
    public float fadeinDuration = 0.1f;

    // durée de disparition de la flamme
    [Range(0f, 0.3f)]
    public float fadeoutDuration = 0.3f;


    private float age = 0f;
     private Vector3 sourcePosition;

    // durée supplémentaire de visibilité (golden ou power bomb)
    private float extraDuration = 0f;

    //dernière valeur utilisée pour remplir la heatmap
    int prevPower = 0;

     private float lifeTime = 0f;
     private float expandTime = 0f;

    public ParticleSystem _particles;

    private bool heatRemoved = false;

    private float removeDelay = 0f;

    public override void Awake()
    {
        base.Awake();
        _particles = this.GetComponent<ParticleSystem>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        ResetData();
    }

    public void ResetData()
    {
        age = 0;
        currentPower = 0;
        prevPower = 0;
        extraDuration = 0;
        removeDelay = 0;
        heatRemoved = false;

        if (_particles != null)
        {
            _particles.Stop();
            ParticleSystem.SubEmittersModule sub = _particles.subEmitters;
            ParticleSystem sub_part = sub.GetSubEmitterSystem(0);
            sub_part.Stop();
        }
    }

    private void Update()
    {

        age += Time.deltaTime;

        // suppression de la chaleur un peu avant la fin de a visibilité de l'explosion
        if (age >= lifeTime - fadeoutDuration)
        {
            if (!heatRemoved) RemoveHeat();
        }


        if (age >= lifeTime)
        {
            //Remove();
            Disable();
        }
        else
        {

            if (age <= expandTime)
            {
                ParticleSystem.MainModule main = _particles.main;

                main.startLifetime = lifeTime - age;
                main.startSize = 1f;

                if (currentPower < endPower)
                {
                    currentPower = Mathf.Lerp(startPower, endPower, age / expandTime);
                    UpdateLength();
                    if (!heatRemoved)
                    {
                        UpdateHeat();
                    }
                }
                else if (_particles.isPlaying)
                {
                    _particles.Stop();
                }
            }
        }

    }

    private void OnDrawGizmos()
    {
        if(currentPower > 0)
        {
            Gizmos.color = Color.yellow;
            for (int i = 0; i <= currentPower; i++)
            {
                Vector3 pos = sourcePosition + direction * i;
                Gizmos.DrawWireCube(pos, Vector3.one * 0.9f);
            }
        }
        
    }

    private void RemoveHeat()
    {
        if (heatRemoved) return;

        heatRemoved = true;

        // on remet le prevPower à zéro, pour repartir du début de la flamme
        prevPower = 0;

        // suppression de la chaleur sur toute la longueur de l'explosion
        UpdateHeat(-1);

        /*
        // suppression de la chaleur au centre de l'explosion
        gameSettings.level.AddHeat(CellX, CellZ, -1);

        // reset de la valeur de la case du centre de l'explosion
        ElementType t = gameSettings.level.GetCellType(CellX, CellZ);
        if (t == ElementType.Explosion || t == ElementType.Fire)
        {
            //Debug.Log("clear fire >");
            gameSettings.level.SetCellElement(sourcePosition, null);
        }
        */
    }

    private void UpdateHeat(int heatValue = 1)
    {
        if (currentPower == 0 || LevelBuilder.grid == null) return;

        int pow = Mathf.RoundToInt(currentPower);
        if (prevPower == pow) return;

        for (int i = prevPower + 1; i <= pow; i++)
        {
            Tile tile = LevelBuilder.grid.GetGridObject(sourcePosition + direction * i);

            if (tile.item is IDestroyable)
            {
                //Debug.Log("auto Boom "+ tile.item.name);
                ((IDestroyable)tile.item).Destroy(0.05f);
            }

            //gameSettings.level.AddHeat((int)(CellX + i * direction.x), CellZ, heatValue);
        }


        prevPower = pow;
    }

    

    void UpdateLength()
    {
        transform.localPosition = sourcePosition + direction * currentPower;
    }

    private float ComputeLimit(float maxPower)
    {
        float limitPower = 0;

        for (int i = 1; i <= maxPower; i++)
        {
            Tile tile = LevelBuilder.grid.GetGridObject(sourcePosition + direction*i);

            if (tile.type >= ElementType.Block)
            {
                // Allow destruction of nearby objects
                if (tile.item is IDestroyable)
                {
                    limitPower++;
                }
                break;
            }
            limitPower++;
        }

        return limitPower;
    }

    //TODO : il faudrait que ça soit exécuté côté client
    public void InitServer(Vector3 position, Vector3 direction, float maxPower, float extraTime = 0f)
    {
        Init(position, direction, maxPower, extraTime);
        RpcInit(position, direction, maxPower, extraTime);
    }

    private void Init(Vector3 startPosition, Vector3 direction, float maxPower, float extraTime)
    {
        
        sourcePosition = startPosition;

        this.direction = direction;

        float globalInstantPowerValue = 1f; // TODO : rendre ce paramètre global dépendant des réglages de la map, ou bien s'en débarrasser

        float limitPower = ComputeLimit(maxPower);

        startPower = Mathf.Min(Mathf.Max(instantFire, globalInstantPowerValue), limitPower);
        currentPower = 0f;
        age = 0f;
        this.endPower = limitPower + cellFilling;

        extraDuration = extraTime;

        lifeTime = Mathf.Max(duration, maxPower * expansionCellTime) + extraDuration;
        expandTime = Mathf.Max(fadeinDuration, limitPower * expansionCellTime);

        expandTime = Mathf.Min(expandTime, lifeTime - fadeoutDuration);
        _particles.Stop();

        ParticleSystem.MainModule main = _particles.main;

        main.startLifetime = lifeTime - fadeinDuration * 1.5f;
        main.startSize = 1f + maxPower * 0.05f;

        _particles.Play();

       
    }

    [ClientRpc]
    private void RpcInit(Vector3 position, Vector3 direction, float maxPower, float extraTime)
    {
        Init(position, direction, maxPower, extraTime);
    }

    //TODO : voir avec le prefab dans l'autre projet pour chopper le setup correct du callback
    public void SetSparks(bool state)
    {
        ParticleSystem.SubEmittersModule sub = _particles.subEmitters;
        ParticleSystem sub_part = sub.GetSubEmitterSystem(0);
        sub_part.gameObject.SetActive(state);

        ParticleSystem.MainModule main = _particles.main;
        if (state)
        {
            main.stopAction = ParticleSystemStopAction.Callback;
            removeDelay = 0;
        }
        else
        {
            main.stopAction = ParticleSystemStopAction.Callback;
            removeDelay = 1f;
        }

    }

    public void OnParticleSystemStopped()
    {
        if (removeDelay == 0)
        {
            Disable();
        }
        else
        {
            Invoke("Disable", removeDelay); 
        }

    }

    public override void Disable()
    {

        RemoveHeat();

        ResetData();


        base.Disable();
    }
}

