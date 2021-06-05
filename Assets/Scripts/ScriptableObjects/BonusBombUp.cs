using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableAction/BonusBehaviour/BonusBombUp", fileName = "BonusBombUp")]
public class BonusBombUp : BonusBehaviour<ItemBonus, PlayerBombDropper>
{

    public override void PerformAction(ItemBonus obj, PlayerBombDropper player)
    {
        PlayerBombDropper dropper = player.GetComponent<PlayerBombDropper>();
        dropper.RpcIncrementBombMax();
    }

}
