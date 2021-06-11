using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerDiseaseManager : MonoBehaviour
{
    Timer timer;
    Disease currentDisease;
    PlayerEntity player;

    public void Init(PlayerEntity player)
    {
        this.player = player;
        timer = GetComponent<Timer>();
        timer.onTimerEnd += EndDisease;
    }

    public void StartDisease(Disease disease)
    {

        currentDisease = disease;
        disease.PerformAction(player);
        timer.DelayedStart(currentDisease.duration);
        Debug.Log("start disease" + currentDisease.ToString());

    }

    public void EndDisease()
    {
        Debug.Log("end disease" + currentDisease.ToString());
        currentDisease.UnPerformAction(player);
        currentDisease = null;

        //timer.onTimerEnd -= EndDisease;
    }

    public void SpreadDisease(PlayerEntity[] nearbyPlayerEntity)
    {
        
    }
}
   


