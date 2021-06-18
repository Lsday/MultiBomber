using UnityEngine;

public class ItemBonus : ItemPlayerModifier, IDestroyable
{

    
    public Renderer rendererObject;

    private MaterialPropertyBlock propertyBlock;

    public override void Loot(PlayerEntity playerEntity)
    {
        BonusBehaviour<PlayerEntity> bonusBehaviour = scriptableAction as BonusBehaviour<PlayerEntity>;
        bonusBehaviour.PerformAction(playerEntity);
        playerEntity.playerBonusPickUp.AddItem(bonusBehaviour);
        base.Loot(playerEntity);
    }

    public void SetBonus(BonusBehaviour<PlayerEntity> bonus)
    {
        scriptableAction = bonus;

        if(bonus != null)
        {
            UpdateTexture(bonus.sprite);
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

        if (propertyBlock == null)
            propertyBlock = new MaterialPropertyBlock();

        propertyBlock.SetVector("_SpriteUV", uv);
        propertyBlock.SetFloat("_NoiseValue", 0);
        rendererObject.SetPropertyBlock(propertyBlock);
    }
}

