using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBonusPickUp : MonoBehaviour
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

   
}
