using UnityEngine;
using System;

[CreateAssetMenu(fileName = "IntVar", menuName = "SO Variable/String Var", order = 3)]
public class SO_String : SO_Variable<string>
{
    public override string ToString(){
        return this.value.ToString();
    }

    public SO_String GetClone(){
        return this.PrepareClone() as SO_String;
    }
}