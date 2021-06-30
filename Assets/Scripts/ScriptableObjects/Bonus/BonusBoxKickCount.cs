using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableAction/Bonus/Bonus Box Kick", fileName = "BonusBoxKickCount")]
public class BonusBoxKickCount : BonusBehaviour<PlayerEntity>
{
    public int kickPower;
    public override void PerformAction(PlayerEntity player)
    {
        player.playerCollisions.RpcModifyBoxKickPower(kickPower);
    }
}
