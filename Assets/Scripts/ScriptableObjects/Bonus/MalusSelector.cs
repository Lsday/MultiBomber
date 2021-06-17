using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableAction/Bonus/MalusSelector", fileName = "MalusSelector")]
public class MalusSelector : BonusBehaviour<PlayerEntity> 
{
    public Disease[] diseases;

    public override void PerformAction(PlayerEntity player)
    {
        int rnd = UnityEngine.Random.Range(0, diseases.Length);

        player.playerDiseaseManager.StartDisease(diseases[rnd], diseases[rnd].duration);
    }

}

