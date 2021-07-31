using System.Collections;
using UnityEngine;
using Mirror;

public class ItemBox : ItemKickable, IDestroyable
{
    //public BonusDropBehaviour bonusDropBehaviour;
    //public ItemBonus bonusToDrop;
    public BonusBehaviour<PlayerEntity> bonus;

    bool destroyedTriggered = false;

    public float explosionScale = 1.3f;

    private float explodeProgress = 0f;
    float destructionDuration = 0.5f;

    public Renderer rendererObject;
    private MaterialPropertyBlock propertyBlock;

    void OnEnable()
    {
        ResetVariables();
    }

    void ResetVariables()
    {
        if (propertyBlock == null)
            propertyBlock = new MaterialPropertyBlock();

        myTransform.localScale = Vector3.one;
        explodeProgress = 0;
        rendererObject.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        propertyBlock.SetFloat("_NoiseValue", 0);

        float rr = LevelBuilder.instance.toggleBoxes ? 0.2f : 1f;
        if (LevelBuilder.instance.toggleBoxes)
        {
            transform.localScale = new Vector3(0.9f, 1f, 1f);
        }
        else
        {
            transform.localScale = new Vector3(1f, 1f, 0.9f);
        }
        propertyBlock.SetFloat("_RandomValue", Random.Range(0.7f, 1f)*rr); // set random color
        rendererObject.SetPropertyBlock(propertyBlock);
    }
    private IEnumerator UpdateMaterialData()
    {
        while (destroyedTriggered)
        {
            explodeProgress += Time.fixedDeltaTime / destructionDuration;
            propertyBlock.SetFloat("_NoiseValue", explodeProgress);
            myTransform.localScale = Vector3.one * Mathf.Lerp(1f, explosionScale, explodeProgress / destructionDuration);
            rendererObject.SetPropertyBlock(propertyBlock);
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }
    }

    void StartDestroyAnimation(float duration)
    {
        ResetVariables();
        if (duration == 0) return;
        // disable shadows when destroying a bonus
        rendererObject.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        // set the destruction time
        destructionDuration = duration;
        
        StartCoroutine(this.UpdateMaterialData());
    }

    public override void Destroy()
    {
        destroyedTriggered = false;

        //Debug.Log(gameObject.name + "Box Destroyed");

        if(bonus != null)
        {
            ItemBonus itemBonus = PoolingSystem.instance.GetPoolObject(ItemsType.BONUS, position2D ) as ItemBonus;
            itemBonus.SetBonus(bonus);
        }
       
        // reset bonus
        bonus = null;

        base.Destroy();
    }

    [Command]
    public void CmdInitDestroy(float delay, float endDelay)
    {
        RpcInitDestroy(delay, endDelay);
    }

    [ClientRpc]
    public void RpcInitDestroy(float delay, float endDelay)
    {
        InitDestroy(delay, endDelay);
    }

    public void InitDestroy(float delay = 0f,float fireEndDelay = 0f)
    {
        if (destroyedTriggered) return;

        delay += fireEndDelay;

        if (delay > 0)
        {
            destroyedTriggered = true;
            Debug.Log("Invoke destroy " + name);
            Invoke("Destroy", delay);

            StartDestroyAnimation(fireEndDelay);
            return;
        }
        Debug.Log("Direct destroy " + name);
        Destroy();
    }

    public override string ToString()
    {
        return bonus == null ? type.ToString() : "Box:" + bonus.name;

    }
}
