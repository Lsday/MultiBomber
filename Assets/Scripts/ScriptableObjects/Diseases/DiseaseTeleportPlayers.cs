using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableAction/Diseases/DiseaseTeleportPlayers", fileName = "DiseaseTeleportPlayers")]

public class DiseaseTeleportPlayers : Disease
{
    
    public override void PerformAction(PlayerEntity player)
    {
        int rnd;
        PlayerEntity opositePlayer;
        

        do
        {
            rnd = UnityEngine.Random.Range(0, PlayerEntity.instancesList.Count);
            opositePlayer = PlayerEntity.instancesList[rnd];
        } 
        while (opositePlayer == player);
       
        Vector3 previousOpositePlayerPos = PlayerEntity.instancesList[rnd].transform.position;

        opositePlayer.playerMovement.Teleport(player.transform.position);

        player.playerMovement.Teleport(previousOpositePlayerPos);
    }

    public override void UnPerformAction(PlayerEntity player)
    {
       
    }

}

