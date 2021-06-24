using System.Collections.Generic;
using Mirror;
using UnityEngine;


public class PrefabPoolManager
{
    [Header("Settings")]
    public int startSize = 20;
    public int maxSize = 100;
    public GameObject prefab;
    public Transform parent;

    [Header("Debug")]
    [SerializeField] Queue<PoolableObject> pool;
    [SerializeField] int currentCount;


    public void Init(GameObject prefab , int startCount,int maxCount, Transform parent)
    {
        this.startSize = startCount;
        this.maxSize = maxCount;
        this.prefab = prefab;
        this.parent = parent;

        InitializePool();

        NetworkClient.RegisterPrefab(prefab, SpawnHandler, UnspawnHandler);
    }

    public void Destroy()
    {
        NetworkClient.UnregisterPrefab(prefab);
    }

    private void InitializePool()
    {
        pool = new Queue<PoolableObject>();
        for (int i = 0; i < startSize; i++)
        {
            PoolableObject next = CreateNew();

            pool.Enqueue(next);
        }
    }

    PoolableObject CreateNew()
    {
        if (currentCount > maxSize)
        {
            Debug.LogError($"Pool has reached max size of {maxSize}");
            return null;
        }

        // use this object as parent so that objects dont crowd hierarchy
        
        GameObject next = GameObject.Instantiate(prefab, parent);
        next.name = $"{prefab.name}_pooled_{currentCount}";
        next.SetActive(false);
        currentCount++;
        PoolableObject obj = next.GetComponent<PoolableObject>();
        obj.poolManager = this;
        return obj;
    }

    // used by NetworkClient.RegisterPrefab
    GameObject SpawnHandler(SpawnMessage msg)
    {
        return GetFromPool(msg.position, msg.rotation);
    }

    // used by NetworkClient.RegisterPrefab
    void UnspawnHandler(GameObject spawned)
    {
        PutBackInPool(spawned);
    }

    /// 
    /// Used to take Object from Pool.
    /// Should be used on server to get the next Object
    /// Used on client by NetworkClient to spawn objects
    /// 
    /// 
    /// 
    /// 
    public GameObject GetFromPool(Vector3 position, Quaternion rotation)
    {
        PoolableObject next = PickFromPool();
        
        // CreateNew might return null if max size is reached
        if (next == null) { return null; }

        // set position/rotation and set active
        next.transform.position = position;
        next.transform.rotation = rotation;
        next.gameObject.SetActive(true);
        return next.gameObject;
    }

    public PoolableObject PickFromPool()
    {
        PoolableObject next = pool.Count > 0
            ? pool.Dequeue() // take from pool
            : CreateNew(); // create new because pool is empty

        // CreateNew might return null if max size is reached
        if (next == null) { return null; }
        return next;
    }

    /// 
    /// Used to put object back into pool so they can b
    /// Should be used on server after unspawning an object
    /// Used on client by NetworkClient to unspawn objects
    /// 
    /// 
    public void PutBackInPool(PoolableObject spawned)
    {
        // disable object
        spawned.gameObject.SetActive(false);

        // add back to pool
        pool.Enqueue(spawned);
    }

    public void PutBackInPool(GameObject spawned)
    {
        // disable object
        spawned.SetActive(false);

        // add back to pool
        pool.Enqueue(spawned.GetComponent<PoolableObject>());
    }
}
