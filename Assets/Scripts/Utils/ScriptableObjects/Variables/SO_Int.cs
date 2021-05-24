using UnityEngine;

[CreateAssetMenu(fileName = "IntVar", menuName = "SO Variable/Int Var", order = 2)]
public class SO_Int : SO_Variable<int>
{
    public bool resetOnAwake = false;
    public int resetValue = 0;

    void OnEnable()
    {
        if (resetOnAwake)   
        {
            value = resetValue;
        }
    }
    public override string ToString(){
        return value.ToString();
    }

    public SO_Int GetClone(){
        return PrepareClone() as SO_Int;
    }
}