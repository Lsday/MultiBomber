using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerDiseaseManager : NetworkBehaviour
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
        RpcStartDisease(disease);
    }

    [ClientRpc]
    private void RpcStartDisease( Disease disease)
    {
        
        currentDisease = disease;
        disease.PerformAction(player);
        timer.DelayedStart(currentDisease.duration);
        Debug.Log("Start of " + currentDisease.ToString());
    }

    public void EndDisease()
    {
        Debug.Log("End of " + currentDisease.ToString());
        currentDisease.UnPerformAction(player);
        currentDisease = null;
    }

    public void SpreadDisease(PlayerEntity[] nearbyPlayerEntity)
    {
        
    }
}
   


