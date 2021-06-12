using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Timer : MonoBehaviour
{
    public Action onTimerEnd;
    [SerializeField] float duration;
    [SerializeField] bool triggerWhenDestroy;
    public float elapsedTime;

    //public void Start() => StartCoroutine(StartTimer(duration));

    public void StartTimer() => StartCoroutine(StartTimerCoroutine(duration));

    public void StartTimer(float duration)
    {
        if (Mathf.Approximately(duration, 0))
        {
            EndTimerEarly();
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(StartTimerCoroutine(duration));
        }
    } 

    private  IEnumerator StartTimerCoroutine(float duration)
    {
        elapsedTime = Time.time;
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
