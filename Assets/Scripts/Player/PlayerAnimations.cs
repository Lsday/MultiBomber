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

    //TODO

    //anim d'�tourdissement = le player est bloqu� sur place le temps de l'anim
    //_animator.SetBool(_stunHash, true);

    //anim coup de poing = le player est bloqu� sur place le temps du coup de poing (0.3s)
    //_animator.SetTrigger(_punchHash);

    // ==> pr�voir un Timer qui permet de d�finir une dur�e d'immobilisation du joueur = d�sactivation temporaire des inputs
}
