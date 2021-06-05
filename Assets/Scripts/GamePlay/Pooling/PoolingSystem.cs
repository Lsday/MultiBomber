using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum ItemsType
{
    BOX,
    BOMB,
    BONUS,
    FLAMES
}



public class PoolingSystem : NetworkBehaviour
{
    public static PoolingSystem instance;
    public Dictionary<ItemsType, List<ItemBase>> pooledObjects = new Dictionary<ItemsType, List<ItemBase>>();
 
    // TODO : faire un système qui identifie les objets par leur ID interne

    #region Box
    public GameObject boxPrefabToPool;
    public int boxAmountToPool;
    #endregion

    #region Bomb
    public GameObject bombPrefabToPool;
    public int bombAmountToPool;
    #endregion

    #region Bonus
    public GameObject bonusPrefabToPool;
    public int bonusAmountToPool;
    #endregion

    #region Flames
    public GameObject flamesPrefabToPool;
    public int flamesAmountToPool;
    #endregion

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        Init();
    }

    public void Init()
    {

        pooledObjects.Add(ItemsType.BOX, new List<ItemBase>());
        pooledObjects.Add(ItemsType.BOMB, new List<ItemBase>());
        pooledObjects.Add(ItemsType.BONUS, new List<ItemBase>());
        pooledObjects.Add(ItemsType.FLAMES, new List<ItemBase>());

        if (isServer)
        {
            ItemBase obj;
            for (int i = 0; i < boxAmountToPool; i++)
            {
                obj = Instantiate(boxPrefabToPool).GetComponent<ItemBase>();
                pooledObjects[ItemsType.BOX].Add(obj);
            }

            for (int i = 0; i < bombAmountToPool; i++)
            {
                obj = Instantiate(bombPrefabToPool).GetComponent<ItemBase>();
                pooledObjects[ItemsType.BOMB].Add(obj);
            }

            for (int i = 0; i < bonusAmountToPool; i++)
            {
                obj = Instantiate(bonusPrefabToPool).GetComponent<ItemBase>();
                pooledObjects[ItemsType.BONUS].Add(obj);
            }

            for (int i = 0; i < flamesAmountToPool; i++)
            {
                obj = Instantiate(flamesPrefabToPool).GetComponent<ItemBase>();
                pooledObjects[ItemsType.FLAMES].Add(obj);
            }
        }
        
    }

    public ItemBase GetPoolObject(ItemsType type)
    {
        if (!pooledObjects.ContainsKey(type)) return null;

        for (int i = 0; i < pooledObjects[type].Count; i++)
        {
            if (!pooledObjects[type][i].isActive)
            {
                pooledObjects[type][i].Enable();
                return pooledObjects[type][i];
            }
        }

        return null;
    }

}
