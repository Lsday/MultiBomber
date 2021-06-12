using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableAction/Diseases/DiseaseMoveCantStop", fileName = "DiseaseMoveCantStop")]

public class DiseaseMoveCantStop : Disease
{
    public Filter filter;
    public override void PerformAction(PlayerEntity player)
    {
        player.playerMovement.currentFilter = filter;
    }

    public override void UnPerformAction(PlayerEntity player)
    {
        player.playerMovement.currentFilter = null;
    }

}

