using UnityEngine;
using Mirror;

public class PoolableObject : NetworkBehaviour
{
    public static Vector3 hiddenPosition = new Vector3(0, -10, 0);

    public PrefabPoolManager poolManager;

    NetworkTransform networkTransform;
    protected Transform myTransform;

    public virtual void Awake()
    {
        myTransform = transform;
        networkTransform = GetComponent<NetworkTransform>();
    }

    public virtual void Disable()
    {
        //Debug.Log("Disable " + name);

        //

        // tell server to send ObjectDestroyMessage, which will call UnspawnHandler on client
        if (isServer)
        {
            poolManager.PutBackInPool(this.gameObject);
            //Debug.Log("Call UNSPAWN for "+name);
            NetworkServer.UnSpawn(this.gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
        
 
    }

    [Server]
    public virtual void Teleport(Vector3 position)
    {
        if (isServer)
        {
            networkTransform.ServerTeleport(position);
        }
        else
        {
            myTransform.position = position;
        }
    }

}
