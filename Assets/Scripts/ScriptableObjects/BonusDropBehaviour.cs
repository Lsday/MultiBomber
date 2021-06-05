using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableAction/Bonus DropBehaviour", fileName = "BonusDropBehaviour")]
public class BonusDropBehaviour : DropBehaviour<ItemBox>
{

    public override void PerformAction(ItemBox obj)
    {


        Debug.Log("BonusDropBehaviour");
        ItemBonus itemBonus = PoolingSystem.instance.GetPoolObject(ItemsType.BONUS) as ItemBonus;
        itemBonus.Teleport(obj.transform.position);

    }

}
