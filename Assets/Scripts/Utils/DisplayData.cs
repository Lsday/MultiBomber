using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayData : MonoBehaviour
{
    public SO_Int physicalDevicesCount;
    public SO_Int localPlayersCount;
    public SO_Int totalPlayersCount;


    private void OnGUI()
    {
        GUI.Label(new Rect(0, 200, 500, 25), "Physical input devices : " + physicalDevicesCount.value.ToString());
        GUI.Label(new Rect(0, 225, 500, 25), "Local players : " + localPlayersCount.value.ToString());
        GUI.Label(new Rect(0, 250, 500, 25), "Total players : " + totalPlayersCount.value.ToString());

        string lst = "";
        for(int i = 0; i < PlayerEntity.instancesList.Count; i++)
        {
            lst += PlayerEntity.instancesList[i].debugName + "\n";
        }

        GUI.Label(new Rect(0, 275, 500, 300), lst);
    }
}
