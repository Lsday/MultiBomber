using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableAction/Filter/FilterCantStopMovement", fileName = "FilterCantStopMovement")]

public class FilterCantStopMovement : Filter, IFilterVector
{

    public void FilterVector(Vector2 input, Vector2 currentDirection, out Vector2 output)
    {
        if(input.x == 0 && input.y == 0)
        {
            output = currentDirection;
        }
        else
        {
            output = input;
        }
    }
}
