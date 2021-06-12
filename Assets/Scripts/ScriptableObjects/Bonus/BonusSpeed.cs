using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableAction/Bonus/BonusSpeed", fileName = "BonusSpeed")]
public class BonusSpeed : BonusBehaviour<PlayerEntity>
{
    public int speedModifier;
    public override void PerformAction(PlayerEntity playerEntity)
    {
       playerEntity.playerMovement.RpcModifySpeed(speedModifier);
    }
}
