using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerDiseaseManager : MonoBehaviour
{
    Timer timer;
    
    public DiseaseBehaviour[] diseaseBehaviours;

    DiseaseBehaviour actualDisease;
    PlayerEntity player;

    public void Init(PlayerEntity player)
    {
        this.player = player;
        timer = GetComponent<Timer>();
        timer.onTimerEnd += EndDisease;
    }

    public void StartDisease()
    {
        int rnd = UnityEngine.Random.Range(0, diseaseBehaviours.Length);

        actualDisease = diseaseBehaviours[rnd];
        actualDisease.PerformAction(player);
        timer.DelayedStart(actualDisease.duration);
        Debug.Log("start disease" + actualDisease.ToString());
        
    }

    public void EndDisease()
    {
        Debug.Log("end disease" + actualDisease.ToString());
        ((IActionReverse)actualDisease).UnPerformAction(player);
        actualDisease = null;
        timer.onTimerEnd -= EndDisease;
    }

    public void SpreadDisease(PlayerEntity[] nearbyPlayerEntity)
    {
        
    }
}
   


