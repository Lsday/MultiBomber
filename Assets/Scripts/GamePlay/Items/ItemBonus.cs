using UnityEngine;

public class ItemBonus : ItemBase, ILootable, IDestroyable
{

    public BonusBehaviour<PlayerEntity> bonusBehaviour;
    bool destroyedTriggered = false;


    public void Destroy()
    {
        Disable();
    }

    public void InitDestroy(float delay = 0, float fireEndDelay = 0f)
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

    public void Loot(PlayerEntity playerEntity)
    {
        Debug.Log("Player :"+playerEntity.netId + " Looted : " + bonusBehaviour.name);
        bonusBehaviour.PerformAction(playerEntity);
        playerEntity.playerBonusPickUp.AddItem(bonusBehaviour);

        Disable();

    }

    public override string ToString()
    {
        return bonusBehaviour.name;
    }

}

