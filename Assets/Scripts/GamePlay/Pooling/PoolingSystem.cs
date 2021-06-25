using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//https://mirror-networking.gitbook.io/docs/guides/gameobjects/custom-spawnfunctions
//https://mirror-networking.gitbook.io/docs/community-guides/resources

public enum ItemsType
{
    BOX,
    BOMB,
    BONUS,
    FLAMES,
}
[Serializable]
public struct PrefabData
{
    public ItemsType type;
    public GameObject prefab;
    public int startCount;
    public int maxCount;
}


public class PoolingSystem : NetworkBehaviour
{
    public static PoolingSystem instance;
    public Dictionary<ItemsType, PrefabPoolManager> pooledObjects = new Dictionary<ItemsType, PrefabPoolManager>();

    public PrefabData[] prefabs;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        Init();
    }

    private void OnDestroy()
    {
        for (int i = 0; i < prefabs.Length; i++)
        {
            ItemsType type = prefabs[i].type;
            if (pooledObjects.ContainsKey(type)) {
                pooledObjects[type].Destroy();
            }
        }
    }

    public void Init()
    {
        for(int i=0; i < prefabs.Length; i++)
        {
            PrefabPoolManager manager = new PrefabPoolManager();

            manager.Init(prefabs[i].prefab , prefabs[i].startCount, prefabs[i].maxCount, transform);

            pooledObjects.Add(prefabs[i].type, manager);

        }
    }

    public ItemBase GetPoolObject(ItemsType type, Vector3 position)
    {
        
        if (!pooledObjects.ContainsKey(type)) return null;

        ItemBase go = pooledObjects[type].PickFromPool() as ItemBase;

        if(go == null)
        {
            Debug.LogError("Pool overflow");
            return null;
        }

        go.transform.position = position;
        go.gameObject.SetActive(true);

        NetworkServer.Spawn(go.gameObject);
        
        return go;
    }

}
