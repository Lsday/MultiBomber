using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableAction/Bomb Behaviour", fileName = "BombBehaviour")]
public class BombBehaviour : ScriptableAction<ItemBase>
{
    [SerializeField, TextArea(5, 10)]
    protected string description;

    public override void PerformAction(ItemBase obj)
    {
        Direction direction = Direction.None;

        ItemBomb bomb = obj as ItemBomb;

        //Vector3Int[] directions = { Vector3Int.forward, Vector3Int.left, Vector3Int.right, Vector3Int.back };
        for (int i = 0; i < Globals.Vector3_directions.Length; i++)
        {
            Vector3Int dir = Globals.Vector3_directions[i];

            Tile tile = LevelBuilder.grid.GetGridObject(bomb.parentTile.x + dir.x , bomb.parentTile.y + dir.z );

            if (tile.type < ElementType.Block || tile.item is IDestroyable)
            {
                // this direction is free
                direction |= (Direction)Mathf.Pow(2, i);
            }
        }

        bomb.explosionDirection = direction;

    }
}
