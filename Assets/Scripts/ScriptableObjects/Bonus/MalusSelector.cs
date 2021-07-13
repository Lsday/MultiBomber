using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableAction/Bonus/MalusSelector", fileName = "MalusSelector")]
public class MalusSelector : BonusBehaviour<PlayerEntity> 
{
    public Disease[] diseases;

    public override void PerformAction(PlayerEntity player)
    {
        int rnd = UnityEngine.Random.Range(0, diseases.Length);
        int aliveCount = PlayerEntity.GetAliveCount();

        // do not teleport if only one player is alive on the map
        while (diseases[rnd] is DiseaseTeleportPlayers && aliveCount < 2)
        {
            rnd = UnityEngine.Random.Range(0, diseases.Length);
        }

        player.playerDiseaseManager.StartDisease(diseases[rnd], diseases[rnd].duration);
    }

}

