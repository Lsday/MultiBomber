using UnityEngine;



public abstract class DropBehaviour<T> : ScriptableAction<T>
{
    [SerializeField, TextArea(5, 10)]
    protected string description;

    public GameObject dropObject;
    public Vector3 dropPositionOffset;

    public override void PerformAction(T obj) { }
}

public class DropBehaviour<T1,T2> : ScriptableAction<T1,T2>
{
    [SerializeField, TextArea(5, 10)]
    protected string description;

    public GameObject dropObject;
    public Vector3 dropPositionOffset;

    public override void PerformAction(T1 obj1,T2 obj2) { }
}
