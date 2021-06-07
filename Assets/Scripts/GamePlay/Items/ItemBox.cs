using System.Collections;
using UnityEngine;
using Mirror;

public class ItemBox : ItemBase, IDestroyable
{
    //public BonusDropBehaviour bonusDropBehaviour;
    //public ItemBonus bonusToDrop;
    public BonusBehaviour<PlayerEntity> bonus;

    bool destroyedTriggered = false;

    public void Destroy()
    {
        destroyedTriggered = false;

        //Debug.Log(gameObject.name + "Box Destroyed");

        if(bonus != null)
        {
            ItemBonus itemBonus = PoolingSystem.instance.GetPoolObject(ItemsType.BONUS) as ItemBonus;
            itemBonus.bonusBehaviour = bonus;
            itemBonus.Teleport(transform.position);
        }
        
        //bonusDropBehaviour.PerformAction(this);

        // reset bonus
        bonus = null;

        Disable();
    }

    public void InitDestroy(float delay = 0f,float fireEndDelay = 0f)
    {
        if (destroyedTriggered) return;

        delay += fireEndDelay;

        if (delay > 0)
        {
            destroyedTriggered = true;
            Invoke("Destroy", delay);
            return;
        }

        Destroy();
    }

    public override string ToString()
    {
        return bonus==null ? type.ToString():"Box:"+bonus.name;
    }
}
