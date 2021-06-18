using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public enum InputButtonState
{
    Default,
    ForceON,
    ForceOFF
}

public struct PlayerInputData
{
    public Vector2 movement;
    public Vector2 direction;
    public InputButtonState dropBomb;
    public float speed;
}

public abstract class Filter : ScriptableObject
{
    public abstract void FilterData(ref PlayerInputData dataIn);
}



