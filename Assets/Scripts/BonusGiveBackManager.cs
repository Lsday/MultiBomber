using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusGiveBackManager : MonoBehaviour
{
    public static BonusGiveBackManager instance;
   
    /// <summary>
    /// time in millisecond
    /// </summary>
    [Range(0f,1f)]
    public float giveBackDelay = 0.1f;

    [Range(1, 20)]
    public int giveBackCount = 5;

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
        timer = 0;
    }

    private void Update()
    {

        timer -= Time.deltaTime;

        if (bonusToDispatch.Count > 0 && timer <= 0)
        {
            timer = giveBackDelay;
            Tile[] freeTiles = LevelBuilder.GetFreeTiles();

            for(int i = 0 ; i < giveBackCount ; i++)
            {
                BonusBehaviour<PlayerEntity> bonusBehaviour = bonusToDispatch.Dequeue();
                Vector3 position = Vector3.zero;
                int r;
                do
                {
                    r = Random.Range(0, freeTiles.Length - 1);

                    if (freeTiles[r] != null)
                    {
                        position = LevelBuilder.grid.GetGridObjectWorldCenter(freeTiles[r].x, freeTiles[r].y);
                        freeTiles[r] = null;
                        break;
                    }
                }
                while (freeTiles[r] == null);

                ItemBonus bonus = PoolingSystem.instance.GetPoolObject(ItemsType.BONUS, position) as ItemBonus;
                bonus.SetBonus(bonusBehaviour);

                if (bonusToDispatch.Count == 0) break;
            }
        }
    }
}
