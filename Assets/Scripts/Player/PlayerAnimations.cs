using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{

    public Action OnPlayerDiedAnimEnded;

    int _speedHash = Animator.StringToHash("Speed");
    int _deadHash = Animator.StringToHash("Dead");
    int _stunHash = Animator.StringToHash("Stun");
    int _punchHash = Animator.StringToHash("Punch");

    public Animator animator;
    public PlayerEntity playerEntity;
    public PlayerMovement playerMovement;

    int state = -1;
    float deathDelay;
    float minDeathDelay = 2f;

    float stunDuration = 0f;
    float punchDuration = 0f;

    public void Awake()
    {
        playerEntity = GetComponent<PlayerEntity>();
        playerEntity.OnPlayerDied += PlayerDie;

        // Get the death animation duration
        AnimationClip[] animClips = animator.runtimeAnimatorController.animationClips;
        for (int i = 0; i < animClips.Length; i++)
        {
            if (animClips[i].name == "Death")
            {
                deathDelay = Mathf.Max(minDeathDelay, animClips[i].averageDuration);
            }

            if (animClips[i].name == "Stun")
            {
                stunDuration = animClips[i].averageDuration;
            }

            if (animClips[i].name == "Punch")
            {
                punchDuration = animClips[i].averageDuration;
            }
        }
    }

    void PlayerDie()
    {
        animator.SetBool(_deadHash, true);
        Invoke("PlayerDiedAnimEnded", deathDelay);
    }

    void PlayerDiedAnimEnded()
    {
        animator.SetBool(_deadHash, false);
        OnPlayerDiedAnimEnded?.Invoke();
    }

    public void UpdateAnimations(bool isMoving)
    {
        if (isMoving)
        {
            state = _speedHash;
            animator.SetFloat(_speedHash, playerMovement.Speed);
        }
        else if (state != 0)
        {
            state = 0;
            animator.SetFloat(_speedHash, 0);
        }
    }


    public void Stun(float duration = 0)
    {
        StartCoroutine(PlayStun(duration));
    }

    IEnumerator PlayStun(float duration)
    {
        duration = Mathf.Max(duration, stunDuration);
        animator.SetBool(_stunHash, true);
        playerEntity.SetLockTime(duration);
        yield return new WaitForSeconds(duration);
        animator.SetBool(_stunHash, false);
    }

    public void Punch()
    {
        StartCoroutine(PlayPunch());
    }

    IEnumerator PlayPunch()
    {
        animator.SetTrigger(_punchHash);
        playerEntity.SetLockTime(punchDuration);
        yield return new WaitForSeconds(punchDuration);
        animator.SetBool(_stunHash, false);
    }

}
