using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class ItemBonus : ItemPlayerModifier
{ 

    public override void Loot(PlayerEntity playerEntity)
    {
        BonusBehaviour<PlayerEntity> bonusBehaviour = scriptableAction as BonusBehaviour<PlayerEntity>;
        bonusBehaviour.PerformAction(playerEntity);
        playerEntity.playerBonusPickUp.AddItem(bonusBehaviour);

        base.Loot(playerEntity);
    }

}

