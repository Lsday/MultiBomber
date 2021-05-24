using UnityEngine;

[CreateAssetMenu(fileName = "BoolVar", menuName = "SO Variable/Bool Var", order = 1)]
public class SO_Bool : SO_Variable<bool>
{
    public bool resetOnAwake = false;
    public bool resetValue = false;

    void OnEnable()
    {
        if (resetOnAwake)
        {
            value = resetValue;
        }
    }

    public override string ToString(){
        return this.value.ToString();
    }

    public SO_Bool GetClone(){
        return this.PrepareClone() as SO_Bool;
    }
}