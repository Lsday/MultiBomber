using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableAction/BonusBehaviour/BonusSpeedUp", fileName = "BonusSpeedUp")]
public class BonusSpeedUp : BonusBehaviour<PlayerEntity>
{
    public override void PerformAction(PlayerEntity playerEntity)
    {
       playerEntity.GetComponent<PlayerMovement>().RpcIncreaseSpeed();
    }

}
