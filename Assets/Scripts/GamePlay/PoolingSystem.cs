using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum ItemsType
{
    BOX,
    BOMB,
    BONUS
}

public class PoolingSystem : NetworkBehaviour
{
    public Dictionary<ItemsType, List<ItemBase>> pooledObjects = new Dictionary<ItemsType, List<ItemBase>>();
    //public List<ItemBase> pooledObjects  = new List<ItemBase>();
    public GameObject objectToPool;
    public int amountToPool;
    public Transform parentObject;


    private void Start()
    {
        Init();
    }

    public void Init()
    {

        pooledObjects.Add(ItemsType.BOX, new List<ItemBase>());
        pooledObjects.Add(ItemsType.BOMB, new List<ItemBase>());
        pooledObjects.Add(ItemsType.BONUS, new List<ItemBase>());

        if (isServer)
        {
            ItemBase obj;
            for (int i = 0; i < amountToPool; i++)
            {
                obj = Instantiate(objectToPool).GetComponent<ItemBase>();
                obj.gameObject.SetActive(false);
                pooledObjects[ItemsType.BOX].Add(obj);
                obj.transform.position = new Vector3(0, -10, 0);
                NetworkServer.Spawn(obj.gameObject);
            }
        }
        
    }

    public ItemBase GetPoolObject(ItemsType type)
    {
        for (int i = 0; i < amountToPool; i++)
        {
            if (!pooledObjects[type][i].gameObject.activeInHierarchy)
            {
                pooledObjects[type][i].gameObject.SetActive(true);
                return pooledObjects[type][i];
            }
        }
        return null;
    }


}
