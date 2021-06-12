using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableAction/Diseases/DiseaseTeleportPlayers", fileName = "DiseaseTeleportPlayers")]

public class DiseaseTeleportPlayers : Disease
{
    
    public override void PerformAction(PlayerEntity player)
    {
        int rnd = UnityEngine.Random.Range(0, PlayerEntity.instancesList.Count);

        PlayerEntity opositePlayer = PlayerEntity.instancesList[rnd];
        Vector3 previousOpositePlayerPos = PlayerEntity.instancesList[rnd].transform.position;

        opositePlayer.playerMovement.Teleport(opositePlayer.transform.position);
        player.playerMovement.Teleport(previousOpositePlayerPos);
    }

    public override void UnPerformAction(PlayerEntity player)
    {
       
    }

}

