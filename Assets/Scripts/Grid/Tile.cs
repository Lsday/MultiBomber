using System;
using Mirror;

[Serializable]
public class Tile 
{
    #region Properties

    public int x, y;

    GenericGrid<Tile> grid;
    public ElementType type { get; private set; }
    public ItemBase item { get; private set; }

    public bool IsDirty => throw new NotImplementedException();

    public byte temperature;
    #endregion

    #region Constructor
    public Tile(GenericGrid<Tile> grid, int x, int y, ElementType type = ElementType.Empty)
    {
        this.grid = grid;
        this.x = x;
        this.y = y;
    } 

    #endregion

    public override string ToString()
    {
        return type.ToString();
    }
    public void ClearTile()
    {
        item = null;
        type = ElementType.Empty;
        grid.OnGridObjectModified(this);
    }
    public void SetTile(ItemBase item)
    {
        this.item = item;
        type = item.type;
        grid.OnGridObjectModified(this);
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
public enum ElementType : byte
{
    Empty = 0,
    Player = 1,
    FutureItemSpawn = 29,
    Item = 30,
    Fire = 100,
    Block = 128, // tous les éléments après celui-ci sont identifiés comme bloquants

    Bomb = 150,
    Explosion = 160,
    ExplodingElement = 161,
    Weight = 170,

    Box = 250,
    Wall = 255
}

