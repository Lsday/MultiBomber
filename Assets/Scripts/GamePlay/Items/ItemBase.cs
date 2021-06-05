using Mirror;
using System;
using System.Collections;
using UnityEngine;


public abstract class ItemBase : PoolableObject
{
    public ElementType type;
    public Tile parentTile { get; protected set; }

    #region UnityCallabsk

    protected virtual void OnEnable()
    {
        if (isServer)
        {
            RpcEnableObject();
        }
    }

    protected virtual void OnDisable()
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
        Disable();
        //gameObject.SetActive(false);
    }

    [ClientRpc]
    private void RpcEnableObject()
    {
        Enable();
        //gameObject.SetActive(true);
    }

    [ClientRpc]
    public void RpcPlaceOnTile(Vector3 position)
    {
        Debug.Log("RpcPlaceOnTile execute");
        //parenttile = levelbuilder.grid?.getgridobject(position);
        //parenttile?.settile(this);
        PlaceOnTile(position);
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

    public void PlaceOnTile(Tile tile)
    {
        parentTile = tile;
        parentTile?.SetTile(this);
    }

    public void RemoveFromTile()
    {
        parentTile?.SetTile(null);
        parentTile = null;
    }
    public override void Disable()
    {
        parentTile?.ClearTile();
        base.Disable();

    }

    [Server]
    public override void Teleport(Vector3 position) // TODO : NE DEVRAIT SE FAIRE QUE COTE SERVEUR
    {
        base.Teleport(position);

        if (LevelBuilder.grid == null) return;

        if (isServer)
        {
            RpcPlaceOnTile(position);
        }
        //else
        //{
        //    PlaceOnTile(position);
        //}
    }


}
