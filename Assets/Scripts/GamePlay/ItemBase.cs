using Mirror;
using System;
using System.Collections;
using UnityEngine;


public abstract class ItemBase : NetworkBehaviour
{
    public ElementType type;
    Tile parentTile;
    NetworkTransform networkTransform;


    public override void OnStartClient()
    {
        base.OnStartClient();
        gameObject.SetActive(false);
    }

    #region UnityCallabsk

    private void Awake()
    {
        networkTransform = GetComponent<NetworkTransform>();
    }
    private void OnEnable()
    {
        if (isServer)
        {
            RpcEnableObject();
        }
    }

    private void OnDisable()
    {
        if (isServer)
        {
          RpcDisableObject();
        }

        RemoveFromTile();
        parentTile?.ClearTile();
    }
    #endregion

    #region Mirror Messages
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
    #endregion

    public void PlaceOnTile()
    {
        parentTile = LevelBuilder.grid.GetGridObject(transform.position);
        parentTile?.SetTile(this);
    }

    public void PlaceOnTile(Vector3 position)
    {
        parentTile = LevelBuilder.grid.GetGridObject(position);
        parentTile?.SetTile(this);
    }

    [ClientRpc]
    public void RpcPlaceOnTile(Vector3 position)
    {
        parentTile = LevelBuilder.grid.GetGridObject(position);
        parentTile?.SetTile(this);
    }

    public void PlaceOnTile(Tile tile)
    {
        parentTile = tile;
        parentTile?.SetTile(this);
    }

    public void RemoveFromTile()
    {
        parentTile = null;
        parentTile?.SetTile(null);
    }

   
    public void Teleport(Vector3 position)
    {
        if (isServer)
        {
            networkTransform.ServerTeleport(position);
        }
        else
        {
            networkTransform.enabled = false;
            transform.position = position;
            networkTransform.enabled = true;
        }

        RpcPlaceOnTile(position);
    }



}
