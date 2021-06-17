using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameTimerUI : MonoBehaviour
{
   public TextMeshProUGUI timer;


   public void SetTimer(string text)
   {
       timer.text = text;
   }

}
