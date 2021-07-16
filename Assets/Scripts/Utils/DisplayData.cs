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

        GUI.Label(new Rect(0, Screen.height - 30, 500, 25), "Physical input devices : " + physicalDevicesCount.value.ToString());
        GUI.Label(new Rect(0, Screen.height - 60, 500, 25), "Local players : " + localPlayersCount.value.ToString());
        GUI.Label(new Rect(0, Screen.height - 90, 500, 25), "Total players : " + totalPlayersCount.value.ToString());

        string lst = "";
        for(int i = 0; i < PlayerEntity.instancesList.Count; i++)
        {
            lst += i+") "+PlayerEntity.instancesList[i].debugName + "\n";
        }

        GUI.Label(new Rect(0, Screen.height - (120 + PlayerEntity.instancesList.Count*30), 500, 300), lst);
    }
}
