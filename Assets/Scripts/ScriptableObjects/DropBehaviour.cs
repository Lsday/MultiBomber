using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableAction/DropBehaviour", fileName = "DropBehaviour")]
public class DropBehaviour<T> : ScriptableAction<T>
{
    [SerializeField, TextArea(5, 10)]
    protected string description;

    public GameObject DropObject;
    public Vector3 dropPositionOffset;

    public override void PerformAction(T obj)
    {

    }

}
