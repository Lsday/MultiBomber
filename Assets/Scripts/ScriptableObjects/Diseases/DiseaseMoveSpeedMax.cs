using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableAction/Diseases/DiseaseSpeedMax", fileName = "DiseaseSpeedMax")]

public class DiseaseMoveSpeedMax : Disease
{
    float previousSpeed;

    public override void PerformAction(PlayerEntity player)
    {
        previousSpeed = player.playerMovement.GetSpeed();
        player.playerMovement.SetToSpeedMax();
    }

    public override void UnPerformAction(PlayerEntity player)
    {
        player.playerMovement.SetSpeed(previousSpeed);
    }

}

