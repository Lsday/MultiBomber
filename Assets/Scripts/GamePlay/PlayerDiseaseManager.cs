using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerDiseaseManager : NetworkBehaviour
{
    public Timer deasiseTimer;
    Disease currentDisease;
    PlayerEntity player;

    public void Init(PlayerEntity player)
    {
        this.player = player;
        deasiseTimer = GetComponent<Timer>();
        deasiseTimer.onTimerEnd += EndDisease;
    }

   
    public void StartDisease(Disease disease, float diseasTimer)
    {
        RpcStartDisease(disease,  diseasTimer);
    }

    [ClientRpc]
    private void RpcStartDisease(Disease disease, float diseasTimer)
    {
        currentDisease = disease;
        deasiseTimer.StopTimer();
        deasiseTimer.StartTimer(diseasTimer);
        disease.PerformAction(player);
        Debug.Log("Start of " + currentDisease.ToString());
    }

    public void EndDisease()
    {
        Debug.Log("End of " + currentDisease.ToString());
        currentDisease.UnPerformAction(player);
        currentDisease = null;
    }

    public void SpreadDisease(PlayerEntity playerTarget)
    {
        float elapsedDiseaseTime = Time.time - deasiseTimer.elapsedTime;

        if (playerTarget.playerDiseaseManager.currentDisease!=null)
        {
            playerTarget.playerDiseaseManager.EndDisease();
        }
        playerTarget.playerDiseaseManager.StartDisease(currentDisease, elapsedDiseaseTime);

    }
}
   


