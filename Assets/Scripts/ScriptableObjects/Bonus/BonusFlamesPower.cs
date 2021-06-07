using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableAction/BonusBehaviour/BonusFlamesPower", fileName = "BonusFlamesPower")]
public class BonusFlamesPower : BonusBehaviour<PlayerEntity>
{
    public int flamePowerModifier;
    public override void PerformAction(PlayerEntity player)
    {
        player.playerBombDropper.RpcModifyFlamesPower(flamePowerModifier);
    }

}
