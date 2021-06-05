using System.Collections;
using UnityEngine;
using Mirror;

public class ItemBox : ItemBase, IDestroyable
{
    public BonusDropBehaviour bonusDropBehaviour;

  
    public void Destroy(float delay = 0f)
    {
        Debug.Log(gameObject.name + "Box Destroyed");

        bonusDropBehaviour.PerformAction(this);

        Disable();
    }
}
