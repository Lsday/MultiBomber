using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableAction/LootAction", fileName = "LootAction")]
public class DropBehaviour : ScriptableAction
{
    [SerializeField, TextArea(5, 10)]
    protected string description;

    [SerializeField]
    protected GameObject[] loots;

    public int lootAmount;
    public bool randomLoot;
    public float autoDestroyTimer = 20f;

    public override void PerformAction(GameObject obj)
    {
        if (randomLoot)
        {
            int randomItemNumber = Random.Range(0, loots.Length);
            for (int i = 0; i < lootAmount; i++)
            {
                GameObject go = Instantiate(loots[randomItemNumber], obj.transform.position, Quaternion.identity);
                Destroy(go, autoDestroyTimer);
            }
        }
        else
        {
            for (int i = 0; i < loots.Length; i++)
            {
                Instantiate(loots[i], obj.transform.position, Quaternion.identity);
            }
        }

    }
}
