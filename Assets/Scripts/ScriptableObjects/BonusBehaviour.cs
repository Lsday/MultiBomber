using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableAction/BonusBehaviour", fileName = "BonusBehaviour")]
public class BonusBehaviour : ScriptableAction<ItemBase>
{
    [SerializeField, TextArea(5, 10)]
    protected string description;

    
    public override void PerformAction(ItemBase obj)
    {

    }
}
