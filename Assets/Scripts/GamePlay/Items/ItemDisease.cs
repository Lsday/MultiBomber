using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class ItemDisease : ItemPlayerModifier
{
    public override void Loot(PlayerEntity playerEntity)
    {

        playerEntity.playerDiseaseManager.StartDisease();

        base.Loot(playerEntity);
    }
}

