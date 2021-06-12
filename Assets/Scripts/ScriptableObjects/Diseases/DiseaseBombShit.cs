using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableAction/Diseases/DiseaseBombShit", fileName = "DiseaseBombShit")]

public class DiseaseBombShit : Disease
{
    public override void PerformAction(PlayerEntity player)
    {
        player.playerBombDropper.ToggleBombShit();
    }

    public override void UnPerformAction(PlayerEntity player)
    {
        player.playerBombDropper.ToggleBombShit();
    }

}

