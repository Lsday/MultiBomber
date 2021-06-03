using UnityEngine;
public abstract class ScriptableAction<T> : ScriptableObject
{
    public abstract void PerformAction(T obj);

}
