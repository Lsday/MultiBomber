using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableAction/Filter/FilterInvertMovement", fileName = "FilterInvertMovement")]

public class FilterInvertMovement : Filter
{
    public override void FilterData(ref PlayerInputData dataIn)
    {
        dataIn.movement *= -1;
    }
}


