using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBonusPickUp : MonoBehaviour
{
    public List<BonusBehaviour<PlayerEntity>> itemBonusList;

    public void AddItem(BonusBehaviour<PlayerEntity> item)
    {
        itemBonusList.Add(item);
    }

    public void RemoveItem(BonusBehaviour<PlayerEntity> item)
    {
        itemBonusList.Remove(item);
    }

   
}
