using Mirror;
using System;
using System.Collections;
using UnityEngine;


public abstract class ItemBase : PoolableObject
{
    public ElementType type;
    public Tile parentTile { get; protected set; }


    #region UnityCallbacks

    #endregion

    #region Mirror Messages


    public override void OnStartClient()
    {
        base.OnStartClient();
        PlaceOnTile();
    }

    [ClientRpc]
    public void RpcPlaceOnTile(Vector3 position)
    {
        //Debug.Log(name+" RpcPlaceOnTile execute");
        //parenttile = levelbuilder.grid?.getgridobject(position);
        //parenttile?.settile(this);
        PlaceOnTile(position);
    }

    #endregion

    public void PlaceOnTile()
    {
        //Debug.Log(name+" PlaceOnTile");
        parentTile = LevelBuilder.grid.GetGridObject(transform.position);
        parentTile?.SetTile(this);
    }

    public void PlaceOnTile(Vector3 position)
    {
        //Debug.Log(name+" PlaceOnTile");
        parentTile = LevelBuilder.grid.GetGridObject(position);
        parentTile?.SetTile(this);
    }

    public void PlaceOnTile(Tile tile)
    {
        //Debug.Log(name+" PlaceOnTile");
        parentTile = tile;
        parentTile?.SetTile(this);
    }

    // remove the item from its tile only if it is still registered in it
    public void RemoveFromTile()
    {
        //Debug.Log(name + " RemoveFromTile Start");
        if (parentTile != null && parentTile.item == this)
        {
            parentTile.ClearTile();
        }
        parentTile = null;
        //type = ElementType.Empty;
        //Debug.Log(name + "RemoveFromTile End");
    }

    public override void Disable() // DISABLE
    {
        //Debug.Log("Disable");
        RemoveFromTile();
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
        else
        {
            PlaceOnTile(position);
        }
    }

    
}
