using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableAction/BombBehaviour", fileName = "BombBehaviour")]
public class BombBehaviour : ScriptableAction
{
    [SerializeField, TextArea(5, 10)]
    protected string description;

    public int bombRadius;

    public override void PerformAction(GameObject obj)
    {

        ItemBase bomb = obj.GetComponent<ItemBase>();

        Vector3Int[] directions = { Vector3Int.forward, Vector3Int.left, Vector3Int.right, Vector3Int.back };
        for (int i = 0; i < directions.Length; i++)
        {
            int distance = 0;

            for (int j = 0; j < bombRadius; j++)
            {
                Tile tile = LevelBuilder.grid.GetGridObject(bomb.parentTile.x + directions[i].x, bomb.parentTile.y + directions[i].z);

                if (tile.item is IDestroyable)
                {
                    distance++;
                    ((IDestroyable)tile.item).Destroy();
                    break;
                }

                if (tile.playerEntity != null)
                {
                    distance++;
                    ((IKillable)tile.playerEntity).Kill();
                    break;
                }

                if (tile.type == ElementType.Wall)
                    break;
                else
                    distance++;
            }
        }
    }
}
