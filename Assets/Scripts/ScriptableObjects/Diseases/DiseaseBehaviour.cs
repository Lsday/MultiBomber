using UnityEngine;


public abstract class DiseaseBehaviour : ScriptableAction<PlayerEntity>  , IActionReverse
{
    [SerializeField, TextArea(5, 10)]
    protected string description;
    public float duration = 20f;

    public override void PerformAction(PlayerEntity player) { }

    public virtual void UnPerformAction(PlayerEntity player)
    {
        
    }
}

