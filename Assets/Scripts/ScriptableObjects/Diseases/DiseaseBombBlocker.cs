using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableAction/DiseaseBehaviour/DiseaseBombBlocker", fileName = "DiseaseBombBlocker")]

public class DiseaseBombBlocker : Disease
{
    public override void PerformAction(PlayerEntity player)
    {
        player.playerBombDropper.ToggleCanDropBomb();
    }

    public override void UnPerformAction(PlayerEntity player)
    {
        player.playerBombDropper.ToggleCanDropBomb();
    }

}

