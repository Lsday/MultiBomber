using UnityEngine;

[CreateAssetMenu(fileName = "FloatVar", menuName = "SO Variable/Float Var", order = 1)]
public class SO_Float : SO_Variable<float>
{
    public bool resetOnAwake = false;
    public float resetValue = 0;

    void OnEnable()
    {
        if (resetOnAwake)
        {
            value = resetValue;
        }
    }

    public override string ToString(){
        return this.value.ToString("f4");
    }

    public SO_Float GetClone(){
        return this.PrepareClone() as SO_Float;
    }
}