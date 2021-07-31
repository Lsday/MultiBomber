using Mirror;
using System;
using System.Collections;
using UnityEngine;


public abstract class ItemBase : PoolableObject
{
    public ElementType type;
    public Tile parentTile { get; protected set; }


    #region UnityCallbacks


    public Vector3 position2D
    {
        get
        {
            Vector3 pos = transform.position;
            return new Vector3(pos.x, 0, pos.z);
        }
    }

    public void SetRoundPosition()
    {
        myTransform.position = LevelBuilder.grid.GetGridObjectWorldCenter(myTransform.position);
    }
    public override void OnStartClient()
    {
        base.OnStartClient();
        PlaceOnTile();
    }

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

    // remove the item from its tile only if it is still registered in it
    public void RemoveFromTile()
    {
        if (parentTile != null && parentTile.item == this)
        {
            parentTile.ClearTile();
        }
        parentTile = null;
    }

    public override void Disable()
    {
        RemoveFromTile();
        base.Disable();
    }

    public virtual void OnDisable()
    {
        RemoveFromTile();
    }

    public virtual void Destroy()
    {
        Disable();
    }

    #endregion

    #region Mirror Messages

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

    [ClientRpc]
    public void RpcPlaceOnTile(Vector3 position)
    {
        PlaceOnTile(position);
    }

    #endregion

}
