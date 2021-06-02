using System.Collections;
using UnityEngine;


public class ItemBox : ItemBase, IKillable
{

    ScriptableAction bonusAction;

    public void Kill()
    {
        Debug.Log(gameObject.name + " isDead");
        
    }
}
