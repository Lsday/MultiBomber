using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableAction/Filter/InvertMovement", fileName = "InvertMovement")]

public class FilterInvertMovement : Filter , IFilterVector
{
    public void FilterVector(Vector2 input, out Vector2 output)
    {
        output  = input * -1 ;
    }
}


