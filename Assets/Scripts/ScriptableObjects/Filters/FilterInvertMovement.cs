using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableAction/Filter/FilterInvertMovement", fileName = "FilterInvertMovement")]

public class FilterInvertMovement : Filter , IFilterVector
{
    public void FilterVector(Vector2 input, Vector2 currentDirection, out Vector2 output)
    {
        output  = input * -1 ;
    }
}


