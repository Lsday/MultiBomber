using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusDispenser
{
    SO_LevelBonusSettings bonusSettings;
   // BonusBehaviour<PlayerEntity>[] bonus = {BonusBombUp, };

    

    public void AssignBonus(List<ItemBox> boxList, int playersCount, int bonusPercentage)
    {
        bonusSettings.boxesCount = boxList.Count;
        bonusSettings.playersCount = playersCount;
        bonusSettings.bonusPercentage = bonusPercentage;

        SO_LevelBonusSettings.BonusStock[] bonusStocks = bonusSettings.ComputeBonusList();
        if (bonusStocks.Length != boxList.Count)
        {
            Debug.LogError(" BonusCount != BoxCount");
            return;
        }

        int boxIndex = 0;
        for (int i = 0; i < bonusStocks.Length; i++)
        {
            for (int j = 0; j < bonusStocks[i].count; j++)
            {
                //boxList[boxIndex].bonusToDrop.bonusBehaviour = bonusStocks[i].name
                boxIndex++;
            }
        }





    }

}
