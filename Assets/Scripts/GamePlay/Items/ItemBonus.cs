﻿using System.Collections;
using UnityEngine;
using Mirror;

public class ItemBonus : ItemBase, ILootable, IDestroyable
{

    BonusBehaviour bonusBehaviour;
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

    public void Loot()
    {
        Debug.Log("Bonus " + netId + " Looted");
        bonusBehaviour.PerformAction(this);
    }
}
