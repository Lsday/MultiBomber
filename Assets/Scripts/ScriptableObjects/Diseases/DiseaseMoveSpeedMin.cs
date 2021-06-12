using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableAction/Diseases/DiseaseSpeedMin", fileName = "DiseaseSpeedMin")]

public class DiseaseMoveSpeedMin : Disease
{
    float previousSpeed;

    public override void PerformAction(PlayerEntity player)
    {
        previousSpeed = player.playerMovement.GetSpeed();
        player.playerMovement.SetToSpeedMin();
    }

    public override void UnPerformAction(PlayerEntity player)
    {
        player.playerMovement.SetSpeed(previousSpeed);
    }

}

