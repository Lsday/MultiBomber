using System.Collections;
using UnityEngine;


public class ItemBox : ItemBase, IDestroyable
{
    public ScriptableAction DropBehaviour;

    public void Destroy()
    {
        Debug.Log(gameObject.name + "  Box Destroyed");

        //DropBehaviour.PerformAction(gameObject);

        Disable();
    }
}
