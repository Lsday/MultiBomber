using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkinsManager : MonoBehaviour
{
    public static PlayerSkinsManager instance;

    public Color[] colors;

    List<Color> availableColors = new List<Color>();

    void Awake()
    {
        instance = this;
        for (int i = 0; i < colors.Length; i++)
        {
            availableColors.Add(colors[i]);
        }
        availableColors.Reverse();
    }

    public Color PickColor()
    {
        Color color;
        if (availableColors.Count == 0)
        {
             color = UnityEngine.Random.ColorHSV(0, 1, 0, 1, 0.7f, 1f);
        }
        else
        {
            color = availableColors.Pop();
        }
        
        return color;
    }

    public void UnpickColor(Color color)
    {
        availableColors.Add(color);
    }
}
