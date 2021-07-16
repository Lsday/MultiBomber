using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkinsManager : MonoBehaviour
{
    public static PlayerSkinsManager instance;

    public Color[] colors;

    Queue<Color> availableColors = new Queue<Color>();

    
    void Awake()
    {
        instance = this;
        for (int i = 0; i < colors.Length; i++)
        {
            availableColors.Enqueue(colors[i]);
        }
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
            color = availableColors.Dequeue();
        }
        
        return color;
    }

    public void UnpickColor(Color color)
    {
        availableColors.Enqueue(color);
    }
}
