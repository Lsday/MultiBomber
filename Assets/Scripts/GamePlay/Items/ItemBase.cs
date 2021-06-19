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
        if (isServer && !activeState)
        {
            RpcEnableObject();
        }
    }

    protected virtual void OnDisable()
    {
        if (isServer && activeState)
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
       // Debug.Log("RpcPlaceOnTile execute");
        //parenttile = levelbuilder.grid?.getgridobject(position);
        //parenttile?.settile(this);
        PlaceOnTile(position);
    }

    #endregion

    public void PlaceOnTile()
    {
        //Debug.Log("PlaceOnTile");
        parentTile = LevelBuilder.grid.GetGridObject(transform.position);
        parentTile?.SetTile(this);
    }

    public void PlaceOnTile(Vector3 position)
    {
        //Debug.Log("PlaceOnTile");
        parentTile = LevelBuilder.grid.GetGridObject(position);
        parentTile?.SetTile(this);
    }

    public void PlaceOnTile(Tile tile)
    {
        //Debug.Log("PlaceOnTile");
        parentTile = tile;
        parentTile?.SetTile(this);
    }

    // remove the item from its tile only if it is still registered in it
    public void RemoveFromTile()
    {
        //Debug.Log("RemoveFromTile Start" + name);
        if (parentTile != null && parentTile.item == this)
        {
            parentTile.ClearTile();
        }
        parentTile = null;
        //type = ElementType.Empty;
        //Debug.Log("RemoveFromTile End" +  name);
    }

    public override void Disable() // DISABLE
    {
        //Debug.Log("Disable");
        RemoveFromTile();
        base.Disable();
    }

    [Server]
    public override void Teleport(Vector3 position) // TODO : NE DEVRAIT SE FAIRE QUE COTE SERVEUR
    {
        //Debug.Log("Teleport");
        base.Teleport(position);

        if (LevelBuilder.grid == null) return;

        if (isServer && isActive)
        {
            RpcPlaceOnTile(position);
        }
        //else
        //{
        //    PlaceOnTile(position);
        //}
    }

    
}
