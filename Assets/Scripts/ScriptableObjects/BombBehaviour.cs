using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableAction/BombBehaviour", fileName = "BombBehaviour")]
public class BombBehaviour : ScriptableAction<ItemBase>
{
    [SerializeField, TextArea(5, 10)]
    protected string description;

    public GameObject flameParticles;

    public override void PerformAction(ItemBase obj)
    {
        Direction direction = Direction.None;

        ItemBomb bomb = obj as ItemBomb;

        Vector3Int[] directions = { Vector3Int.forward, Vector3Int.left, Vector3Int.right, Vector3Int.back };
        for (int i = 0; i < directions.Length; i++)
        {
            Tile tile = LevelBuilder.grid.GetGridObject(bomb.parentTile.x + directions[i].x , bomb.parentTile.y + directions[i].z );

            if (tile.type < ElementType.Block)
            {
                direction |= (Direction)Mathf.Pow(2, i);
            }

        }

        bomb.explosionDirection = direction;



    }
}
