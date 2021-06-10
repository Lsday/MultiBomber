using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableAction/DiseaseBehaviour/DiseaseBombBlocker", fileName = "DiseaseBombBlocker")]

public class DiseaseBombBlocker : DiseaseBehaviour
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

