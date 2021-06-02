using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// This class receive messages from the input system PlayerInput class
/// which reacts to the human players inputs from the keyboard or gamepads
/// </summary>
public class PhysicalDevice : MonoBehaviour
{
    protected DeviceEntity device;
    protected PlayerInput playerInput;
    public static Action OnSpacePressed;

    public virtual void Start()
    {
        device = GetComponent<DeviceEntity>();
        playerInput = device.GetPhysicalInput();
    }

    public virtual void OnMove(InputValue value)
    {
        Vector2 moveVector = value.Get<Vector2>();

        // send movement vector
        //Debug.Log("Move "+name + moveVector.ToString("f2"));
    }

    public virtual Vector2 GetMoveVector()
    {
        return playerInput.actions["Move"].ReadValue<Vector2>();
    }

    

    public virtual void OnDropBomb(InputValue value)
    {
        Debug.Log("Space BAr Pressed");
        OnSpacePressed?.Invoke();

    }
}
