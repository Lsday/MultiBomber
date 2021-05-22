using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Timer : MonoBehaviour
{
    public Action onTimerEnd;
    [SerializeField] float duration;
    [SerializeField] bool triggerWhenDestroy;

    public void Init() => StartCoroutine(StartTimer());

    private  IEnumerator StartTimer()
    {
        yield return new WaitForSeconds(duration);
        onTimerEnd?.Invoke();
    }

    public void EndTimerEarly()
    {
        StopAllCoroutines();
        onTimerEnd?.Invoke();
    }

    private void OnDestroy()
    {
        if (triggerWhenDestroy)
            onTimerEnd?.Invoke();
    }






}
