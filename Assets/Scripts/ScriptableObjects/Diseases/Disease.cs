using UnityEngine;

public abstract class Disease : ScriptableAction<PlayerEntity>
{
    public float duration;
    public abstract override void PerformAction(PlayerEntity input);

    public abstract void UnPerformAction(PlayerEntity input);
}