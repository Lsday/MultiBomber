using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ItemBonus : ItemPlayerModifier
{

    public AudioClip pick;

    public float explosionScale = 1.3f;

    private float explodeProgress = 0f;
    float destructionDuration = 0.5f;
    
    public Renderer rendererObject;

    private MaterialPropertyBlock propertyBlock;

    [SyncVar(hook = nameof(SyncUV))] Vector4 spriteUV;
    [SyncVar(hook = nameof(SyncColor))] Color burnColor;
    public override void Loot(PlayerEntity playerEntity)
    {
        BonusBehaviour<PlayerEntity> bonusBehaviour = scriptableAction as BonusBehaviour<PlayerEntity>;
        bonusBehaviour.PerformAction(playerEntity);
        playerEntity.playerBonusManager.AddItem(bonusBehaviour);
        base.Loot(playerEntity);
    }

    public void SetBonus(BonusBehaviour<PlayerEntity> bonus)
    {
        ResetVariables();

        scriptableAction = bonus;

        if(bonus != null)
        {
            UpdateTexture(bonus.sprite);
            UpdateColor(bonus.burnColor);
        }
    }

    void UpdateColor(Color color)
    {

        if (isServer)
        {
            burnColor = color;
        }
        else
        {
            SetColor(color);
        }
    }

    void UpdateTexture(Sprite sprite)
    {
       
        float texW = sprite.texture.width;
        float texH = sprite.texture.height;
        Vector4 uv = Vector4.zero;

        uv.x = sprite.rect.x / texW;
        uv.y = sprite.rect.y / texH;
        uv.z = sprite.rect.width / texW;
        uv.w = sprite.rect.height / texH;

        if (isServer)
        {
            spriteUV = uv;
        }
        else
        {
            SetSpriteUV(uv);
        }

    }

    void SetSpriteUV(Vector4 uv)
    {
        if (propertyBlock == null)
            propertyBlock = new MaterialPropertyBlock();

        propertyBlock.SetVector("_SpriteUV", uv);
        propertyBlock.SetFloat("_NoiseValue", 0);
        rendererObject.SetPropertyBlock(propertyBlock);
    }

    void SyncUV(Vector4 oldUV, Vector4 newUV)
    {
        SetSpriteUV(newUV);
    }

    void SyncColor(Color oldColor, Color newColor)
    {
        SetColor(newColor);
    }

    void SetColor(Color color)
    {
        if (propertyBlock == null)
            propertyBlock = new MaterialPropertyBlock();

        propertyBlock.SetColor("_BurnColor", color);
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

    void ResetVariables()
    {
        if (propertyBlock == null)
            propertyBlock = new MaterialPropertyBlock();

        myTransform.localScale = Vector3.one;
        explodeProgress = 0;
        rendererObject.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        propertyBlock.SetFloat("_NoiseValue", 0);
        rendererObject.SetPropertyBlock(propertyBlock);
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

    public override void InitDestroy(float delay = 0, float fireEndDelay = 0f)
    {

        if (destroyedTriggered) return;

        base.InitDestroy(delay, fireEndDelay);

        StartDestroyAnimation(fireEndDelay);
    }
    
    public void Enable()
    {
        ResetVariables();
    }
    
}

