using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableAction/DropBehaviour", fileName = "DropBehaviour")]
public class DropBehaviour : ScriptableAction<ItemBase>
{
    [SerializeField, TextArea(5, 10)]
    protected string description;

    GameObject DropObject;

    public override void PerformAction(ItemBase obj)
    {
        ItemBomb bomb = obj as ItemBomb;

        Vector3Int[] directions = { Vector3Int.forward, Vector3Int.left, Vector3Int.right, Vector3Int.back };

        for (int i = 0; i < directions.Length; i++)
        {

            if ((bomb.explosionDirection & (Direction)Mathf.Pow(2, i)) > 0)
            {
                Flames flames = PoolingSystem.instance.GetPoolObject(ItemsType.FLAMES) as Flames;
                flames.transform.position = bomb.transform.position;
                flames.flamesPower = bomb.bombPower;
                flames.direction = directions[i];
                flames.Teleport(LevelBuilder.grid.GetGridObjectWorldCenter(bomb.parentTile.x, bomb.parentTile.y));
            }
        }
    }
}
