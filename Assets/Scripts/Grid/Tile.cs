using System;
using UnityEngine;

[Serializable]
public class Tile 
{
    #region Properties

    public int x, y;

    GenericGrid<Tile> grid;
    public ElementType type { get; private set; }
    public ItemBase item { get; private set; }

    public PlayerEntity playerEntity{ get; private set; }

    public bool IsDirty => throw new NotImplementedException();

    private byte _temperature = 0;
    public byte temperature {
        get { return _temperature; }

        set
        {
            _temperature = value;
            grid.OnGridObjectModified(this);
        }
    }
    #endregion

    #region Constructor
    public Tile(GenericGrid<Tile> grid, int x, int y)
    {
        this.grid = grid;
        this.x = x;
        this.y = y;
        this.type = ElementType.Empty;
    } 

    #endregion

   
    public override string ToString()
    {
       
        return _temperature.ToString();

        //if (item != null)
        //{
        //    return item.ToString();
        //}
        //return type.ToString() + "("+_temperature+")";
        
    }
    public void ClearTile()
    {
        item = null;
        type = ElementType.Empty;
        grid.OnGridObjectModified(this);
    }
    public void SetTile(ItemBase item)
    {
        bool changed = false;

        if(this.item != item)
        {
            this.item = item;
            changed = true;
        }
        
        if (item != null && type != item.type)
        {
            type = item.type;
            changed = true;
        }

        if (changed)
        {
            grid.OnGridObjectModified(this);
        }
       
    }
    public void SetItem(ItemBase item)
    {
        this.item = item;
        grid.OnGridObjectModified(this);
    }
    public void ClearItem()
    {
        item = null;
        grid.OnGridObjectModified(this);
    }
    public void SetType(ElementType type)
    {
        this.type = type;
        grid.OnGridObjectModified(this);
    }
    public void ClearType()
    {
        type = ElementType.Empty;
        grid.OnGridObjectModified(this);
    }
}


//TODO AJOUTER DES FLAGS
[Flags]
public enum ElementType : int
{
    None = 0,
    Empty = 1 ,
    Player = 1 << 1 ,
    FutureItemSpawn = 1 << 2,
    Item = 1 << 3,
    Fire = 1 << 4,

    Block = 1 << 5, // tous les éléments après celui-ci sont identifiés comme bloquants

    Bomb = 1 << 6,
    Explosion = 1 << 7,
    ExplodingElement = 1 << 8 ,
    Weight = 1 << 9,

    Box = 1 << 10,
    Wall = 1 << 11
}

