using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableAction/Bonus/Bonus BombKick", fileName = "BonusBombKickCount")]
public class BonusBombKickCount : BonusBehaviour<PlayerEntity>
{
    public int kickPower;
    public override void PerformAction(PlayerEntity player)
    {
        player.playerCollisions.RpcModifyBombKickPower(kickPower);
    }
}
