using Mirror;
using System;
using System.Collections;
using UnityEngine;


public abstract class ItemBase : NetworkBehaviour
{
    public ElementType type;
    Tile parentTile;
    private bool isOnMap;
    public bool isNetworkSpawned;

    private void Awake()
    {
        AutoSpawn();
    }

    private void Start()
    {
        parentTile = LevelBuilder.grid.GetGridObject(transform.position);
        parentTile.SetTile(this);
        //parentTile.SetType(type);
       
        AutoSpawn();

    }

    private void OnDisable()
    {
        if (isServer)
        {
            RpcDisableObject();
        }

        parentTile?.ClearTile();
        isOnMap = false;

    }

    [ClientRpc]
    private void RpcDisableObject()
    {
        gameObject.SetActive(false);
    }

    [ClientRpc]
    private void RpcEnableObject()
    {
        gameObject.SetActive(true);
    }

    private void OnEnable()
    {
        AutoSpawn();
        if (isOnMap) return;

        isOnMap = true; 

        

        
            parentTile = LevelBuilder.grid.GetGridObject(transform.position);

            parentTile?.SetTile(this);

            if (isServer)
            {
                RpcEnableObject();
            }

       
    }

    public void AutoSpawn(bool forceSpawn = false)
    {
        if (isServer || forceSpawn)
        {
            if (!isNetworkSpawned)
            {
                NetworkServer.Spawn(gameObject);
                isNetworkSpawned = true;
            }
        }
    }





}
