using Mirror;
using System;
using System.Collections;
using UnityEngine;


public abstract class ItemBase : NetworkBehaviour
{
    public ElementType type;
    Tile parentTile;
    private bool isAlreadySpawned;

    public override void OnStartClient()
    {
        base.OnStartClient();
        parentTile = LevelBuilder.grid.GetGridObject(transform.position);
        parentTile.SetTile(this);
        //parentTile.SetType(type);
        isAlreadySpawned = true;
    }

    private void OnDisable()
    {
        if (isServer)
        {
            RpcDisableObject();
        }

        parentTile.ClearTile();

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
        if (isAlreadySpawned)
        {
            parentTile = LevelBuilder.grid.GetGridObject(transform.position);

            parentTile.SetTile(this);

            if (isServer)
            {
                RpcEnableObject();
            }

        }
    }





}
