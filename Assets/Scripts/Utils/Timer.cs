using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Timer : MonoBehaviour
{
    public Action onTimerEnd;
    [SerializeField] float duration;
    [SerializeField] bool triggerWhenDestroy;

    //public void Start() => StartCoroutine(StartTimer(duration));

    public void StartTimer() => StartCoroutine(StartTimer(duration));

    public void DelayedStart(float duration)
    {
        if (Mathf.Approximately(duration, 0))
        {
            EndTimerEarly();
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(StartTimer(duration));
        }
    } 

    private  IEnumerator StartTimer(float duration)
    {
        yield return new WaitForSeconds(duration);
        onTimerEnd?.Invoke();
    }

    public void EndTimerEarly()
    {
        StopAllCoroutines();
        onTimerEnd?.Invoke();
    }

    public void StopTimer()
    {
        StopAllCoroutines();
    }

    private void OnDisable()
    {
        if (triggerWhenDestroy)
            onTimerEnd?.Invoke();
    }






}
