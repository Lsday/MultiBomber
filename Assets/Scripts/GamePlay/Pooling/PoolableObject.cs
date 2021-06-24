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
        poolManager.PutBackInPool(this.gameObject);

        // tell server to send ObjectDestroyMessage, which will call UnspawnHandler on client
        NetworkServer.UnSpawn(this.gameObject);
 
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
