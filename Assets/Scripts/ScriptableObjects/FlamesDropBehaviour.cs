using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableAction/Fire DropBehaviour", fileName = "FlamesDropBehaviour")]
public class FlamesDropBehaviour : DropBehaviour<ItemBase>
{

    public override void PerformAction(ItemBase obj)
    {
        ItemBomb bomb = obj as ItemBomb;
        
        if (bomb.explosionDirection == Direction.None) // instancie des flammes sur une seule case
        {
            SpawnFlames(bomb.transform.position - dropPositionOffset, Vector3.up, 1);
            return;
        }

        Vector3Int[] directions = { Vector3Int.forward, Vector3Int.left, Vector3Int.right, Vector3Int.back };

        for (int i = 0; i < directions.Length; i++) // instancie des flammes sur les directions données
        {

            if ((bomb.explosionDirection & (Direction)Mathf.Pow(2, i)) > 0)
            {
                SpawnFlames(bomb.transform.position + dropPositionOffset, directions[i], bomb.bombPower);
            }
        }
    }



    void SpawnFlames(Vector3 position , Vector3 direction , float power)
    {
        ItemFlames flames = PoolingSystem.instance.GetPoolObject(ItemsType.FLAMES) as ItemFlames; // TODO : revoir le pooling system pour prendre des prefab en entrée

        flames.InitServer(position, direction,power);

    }
}
