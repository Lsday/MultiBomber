using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolingSystem : NetworkBehaviour
{
    public static PoolingSystem Instance;
    public List<GameObject> pooledObjects;
    public GameObject objectToPool;
    public int amountToPool;
    public Transform parentObject;
    bool initilized = false;
    void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (!isServer)
        {
            NetworkClient.RegisterHandler<RegisterBoxsMessage>(RegisterSpawnedBoxes);
        }
    }

    private void RegisterSpawnedBoxes(RegisterBoxsMessage msg)
    {
        if (isServer) return;

        Debug.Log("RegisterBoxsMessage Receive");

        ItemBox[] boxs = FindObjectsOfType<ItemBox>();

        for (int i = 0; i < boxs.Length; i++)
        {
            GameObject temp = boxs[i].gameObject;
            temp.transform.SetParent(parentObject);
            pooledObjects.Add(temp);
        }
    }

    public void Init()
    {
        if (initilized) return;
        initilized = true;

        if (isServer)
        {
            pooledObjects = new List<GameObject>();
            GameObject obj;
            for (int i = 0; i < amountToPool; i++)
            {
                obj = Instantiate(objectToPool, parentObject, true);
                obj.SetActive(false);
                pooledObjects.Add(obj);
            }
        }
    }

    public GameObject GetPoolObject()
    {
        for (int i = 0; i < amountToPool; i++)
        {
            if (!pooledObjects[i].activeInHierarchy)
            {
                pooledObjects[i].SetActive(true);
                pooledObjects[i].GetComponent<ItemBase>().AutoSpawn(true);
                return pooledObjects[i];
            }
        }
        return null;
    }


}
