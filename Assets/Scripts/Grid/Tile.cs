

using System;

[Serializable]
public class Tile
{
    public int x, y;
    GenericGrid<Tile> grid;
    public ElementType type { get; private set; }

    public Tile(GenericGrid<Tile> grid, int x, int y)
    {
        this.grid = grid;
        this.x = x;
        this.y = y;
    }

    public override string ToString()
    {
        return type.ToString();
    }

    public void SetType(ElementType type)
    {
        this.type = type;
        grid.OnGridObjectModified(this);
    }

    public void GetType(ElementType type)
    {
        this.type = type;
        grid.OnGridObjectModified(this);
    }
}

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

