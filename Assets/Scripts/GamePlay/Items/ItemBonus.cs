using System.Collections;
using UnityEngine;
using Mirror;

public class ItemBonus : ItemBase, ILootable, IDestroyable
{

    BonusBehaviour bonusBehaviour;

    public void Destroy(float delay = 0)
    {
        Disable();
    }

    public void Loot()
    {
        Debug.Log("Bonus " + netId + " Looted");
        bonusBehaviour.PerformAction(this);
    }
}

