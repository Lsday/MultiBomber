using UnityEngine;

internal interface IFilterVector
{
    public void FilterVector(Vector2 inputMovement, Vector2 inputDirection, out Vector2 outputMovement);
}