using Mirror;
using UnityEngine;


public abstract class Disease : ScriptableAction<PlayerEntity>
{
    public float duration;

    public abstract override void PerformAction(PlayerEntity input);

    public abstract void UnPerformAction(PlayerEntity input);
}

public static class DiseaseSerializer
{
    public static void WriteDisease(this NetworkWriter writer, Disease disease)
    {
        writer.WriteString(disease.name);
    }

    public static Disease ReadDisease(this NetworkReader reader)
    {
        // load the same armor by name.  The data will come from the asset in Resources folder
        return (Disease)Resources.Load(reader.ReadString());
    }
}