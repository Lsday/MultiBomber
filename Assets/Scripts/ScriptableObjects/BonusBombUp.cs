using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableAction/BonusBehaviour/BonusBombUp", fileName = "BonusBombUp")]
public class BonusBombUp : BonusBehaviour<PlayerEntity>
{
    public override void PerformAction(PlayerEntity player)
    {
        PlayerBombDropper dropper = player.GetComponent<PlayerBombDropper>();
        dropper.RpcIncrementBombMax();
    }

}
