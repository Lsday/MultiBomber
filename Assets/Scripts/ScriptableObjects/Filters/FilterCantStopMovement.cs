using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableAction/Filter/FilterCantStopMovement", fileName = "FilterCantStopMovement")]

public class FilterCantStopMovement : Filter, IFilterVector
{
    public void FilterVector(Vector2 input, out Vector2 output)
    {
        output = new Vector2();

        if (input.x >= 0)
            output.x = 1;

        if (input.x < 0)
            output.x = -1;

        if (input.y >= 0)
            output.y = 1;

        if (input.y < 0)
            output.y = -1;

       
    }
}
