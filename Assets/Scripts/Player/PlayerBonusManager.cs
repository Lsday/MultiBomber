using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBonusManager : MonoBehaviour
{
    /// <summary>
    /// Conatain all the bonus that the player had already loot
    /// </summary>
    public List<BonusBehaviour<PlayerEntity>> itemBonusList;

    public void AddItem(BonusBehaviour<PlayerEntity> item)
    {
        itemBonusList.Add(item);
    }

    public void RemoveItem(BonusBehaviour<PlayerEntity> item)
    {
        itemBonusList.Remove(item);
    }

    public void GiveAllBonusBack()
    {
        for (int i = 0; i < itemBonusList.Count; i++)
        {
            BonusGiveBackManager.instance.AddBonus(itemBonusList[i]);
        }

        ResetVariables();
    }

    public void ResetVariables()
    {
        itemBonusList.Clear();
    }
}
