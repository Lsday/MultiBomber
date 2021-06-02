using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableAction/BombAction", fileName = "BombAction")]
public class BombAction : ScriptableAction
{
    [SerializeField, TextArea(5, 10)]
    protected string description;

    public override void PerformAction(GameObject obj)
    {

    }
}
