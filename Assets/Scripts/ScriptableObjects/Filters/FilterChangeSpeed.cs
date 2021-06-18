using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableAction/Filter/FilterChangeSpeed", fileName = "FilterChangeSpeed")]

public class FilterChangeSpeed : Filter
{
    public float speed;
    public override void FilterData(ref PlayerInputData dataIn)
    {
        dataIn.speed = speed;
    }
}
