using UnityEngine;

public class ItemPlayerModifier : ItemBase, ILootable, IDestroyable
{

    public ScriptableAction<PlayerEntity> scriptableAction;
    bool destroyedTriggered = false;

    public void Destroy()
    {
        Disable();
    }

    public virtual void InitDestroy(float delay = 0, float fireEndDelay = 0f)
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

    public virtual void Loot(PlayerEntity playerEntity)
    {
        if (scriptableAction)
             Debug.Log("Player :"+ playerEntity.netId + " Looted : " + scriptableAction.name);
            
        Disable();

    }

    public override string ToString()
    {
        if (scriptableAction)
            return scriptableAction.name;

        return name;
        
        
        
    }

}

