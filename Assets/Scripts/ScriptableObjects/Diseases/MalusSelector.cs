using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableAction/DiseaseBehaviour/MalusSelector", fileName = "MalusSelector")]
public class MalusSelector : BonusBehaviour<PlayerEntity> 
{
    public float duration = 20;

    public Disease[] diseases;

    public override void PerformAction(PlayerEntity player)
    {
        int rnd = UnityEngine.Random.Range(0, diseases.Length);

        player.playerDiseaseManager.StartDisease(diseases[rnd]);
    }

    public virtual void UnPerformAction(PlayerEntity player)
    {
        
    }
}

