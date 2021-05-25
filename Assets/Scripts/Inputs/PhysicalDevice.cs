using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PhysicalDevice : MonoBehaviour
{
    protected DeviceEntity device;
    protected PlayerInput playerInput;
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
        // call drop bomb function
        Debug.Log("Drop Bomb");
    }
}
