using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableAction/Filter/FilterCantStopMovement", fileName = "FilterCantStopMovement")]

public class FilterCantStopMovement : Filter
{
    public override void FilterData(ref PlayerInputData dataIn)
    {
        if (dataIn.movement.x == 0 && dataIn.movement.y == 0)
        {
            dataIn.movement = dataIn.direction;
        }
    }
}
