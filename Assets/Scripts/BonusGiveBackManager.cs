using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusGiveBackManager : MonoBehaviour
{
    public static BonusGiveBackManager instance;
   
    /// <summary>
    /// time in millisecond
    /// </summary>
    float giveBackDelay = 0.2f;
    float timer;
    Vector2Int gridDimensions;
    

    Queue<BonusBehaviour<PlayerEntity>> bonusToDispatch = new Queue<BonusBehaviour<PlayerEntity>>();

    private void Awake()
    {
        instance = this;
        timer = giveBackDelay;
        
    }
   
    public void AddBonus(BonusBehaviour<PlayerEntity> bonus)
    {
        bonusToDispatch.Enqueue(bonus);
    }

    private void Update()
    {

        timer -= Time.deltaTime;

        if (bonusToDispatch.Count > 0 && timer <= 0)
        {
            timer = giveBackDelay;
            BonusBehaviour<PlayerEntity> bonusBehaviour = bonusToDispatch.Dequeue();

            ItemBonus bonus =  PoolingSystem.instance.GetPoolObject(ItemsType.BONUS, GetRandomPosition()) as ItemBonus;
            bonus.SetBonus(bonusBehaviour);
        }
    }

    private Vector3 GetRandomPosition()
    {
        if (gridDimensions != null)
        {
            gridDimensions = LevelBuilder.grid.GetGridDimensions();
        }
      
        int randX, randY;

        while (true)
        {
            randX = Random.Range(0, gridDimensions.x);
            randY = Random.Range(0, gridDimensions.y);

            Tile tile = LevelBuilder.grid.GetGridObject(randX, randY); 
            if (tile.item == null && tile.type == ElementType.Empty)
            {
                return LevelBuilder.grid.GetGridObjectWorldCenter(randX, randY);
            }
        }
    }
}
