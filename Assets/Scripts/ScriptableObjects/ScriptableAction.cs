using UnityEngine;
public abstract class ScriptableAction<T1> : ScriptableObject
{
    public abstract void PerformAction(T1 obj1);

}

public abstract class ScriptableAction<T1, T2> : ScriptableObject
{
      public abstract void PerformAction(T1 obj1, T2 obj2);
}
