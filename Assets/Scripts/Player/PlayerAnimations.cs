using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{

    int _speedHash = Animator.StringToHash("Speed");
    int _deadHash = Animator.StringToHash("Dead");
    int _stunHash = Animator.StringToHash("Stun");
    int _punchHash = Animator.StringToHash("Punch");

    public Animator _animator;
    public PlayerEntity playerEntity;
    public PlayerMovement playerMovement;

    int state = -1;

    public void UpdateAnimations(bool isMoving)
    {
        if (isMoving)
        {
            state = _speedHash;
            _animator.SetFloat(_speedHash, playerMovement.Speed);
        }
        else if (state != 0)
        {
            state = 0;
            _animator.SetFloat(_speedHash, 0);
        }
    }

    // anim de mort
    //_animator.SetBool(_deadHash, true);

    //anim d'étourdissement = le player est bloqué sur place le temps de l'anim
    //_animator.SetBool(_stunHash, true);

    //anim coup de poing = le player est bloqué sur place le temps du coup de poing (0.3s)
    //_animator.SetTrigger(_punchHash);

    // ==> prévoir un Timer qui permet de définir une durée d'immobilisation du joueur = désactivation temporaire des inputs
}
