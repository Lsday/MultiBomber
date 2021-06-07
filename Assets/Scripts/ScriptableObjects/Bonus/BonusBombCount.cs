using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableAction/BonusBehaviour/BonusBombStock", fileName = "BonusBombStock")]
public class BonusBombCount : BonusBehaviour<PlayerEntity>
{
    public int bombCountModifier;
    public override void PerformAction(PlayerEntity player)
    {
        player.playerBombDropper.RpcModifyBombCount(bombCountModifier);
    }

}
