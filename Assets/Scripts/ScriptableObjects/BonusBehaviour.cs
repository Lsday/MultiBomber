using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableAction/BonusBehaviour", fileName = "BonusBehaviour")]
public class BonusBehaviour : ScriptableAction
{
    [SerializeField, TextArea(5, 10)]
    protected string description;

    
    public override void PerformAction(GameObject obj)
    {

    }
}

public class BonusBehaviour<T> : ScriptableAction<T>
{
    [SerializeField, TextArea(5, 10)]
    protected string description;


    public override void PerformAction(T obj)
    {

    }
}

public class BonusBehaviour<T1,T2> : ScriptableAction<T1, T2>
{
    [SerializeField, TextArea(5, 10)]
    protected string description;


    public override void PerformAction(T1 obj, T2 obj2)
    {

    }
}
