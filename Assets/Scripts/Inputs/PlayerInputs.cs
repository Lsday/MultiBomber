using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputs : MonoBehaviour
{
    public static List<PlayerInputs> instances = new List<PlayerInputs>();

    public SO_Int ConnectedCount;

    private PlayerInput playerInput;
    public bool connected = true;
    private Gamepad myGamepad;

    public byte localPlayerIndex = 0;
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();

        localPlayerIndex = (byte)instances.Count;

        name += " "+ localPlayerIndex.ToString();
        instances.Add(this);
    }

    void OnDeviceLost()
    {
        connected = false;
        ConnectedCount.value--;
        DetectGamepad();
        Debug.Log("Device Lost "+name+" "+ myGamepad);
    }

    void OnDeviceRegained()
    {
        connected = true;
        ConnectedCount.value++;
        DetectGamepad();
        Debug.Log("Device regained " + name+" "+ myGamepad);
    }

    void DetectGamepad()
    {
        if (playerInput.devices.Count > 0)
        {
            myGamepad = playerInput.devices[0] as Gamepad;
        }
        else
        {
            myGamepad = null;
        }

    }

    void OnControlsChanged()
    {
        // to force refresh on anything connected to the connected players count
        ConnectedCount.Refresh();
        DetectGamepad();
        Debug.Log("control changed " + name);
    }

    public void OnMove(InputValue value)
    {
        Vector2 moveVector = value.Get<Vector2>();

        // send movement vector
        //Debug.Log("Move "+name + moveVector.ToString("f2"));
    }

    public Vector2 GetMoveVector()
    {
        return playerInput.actions["Move"].ReadValue<Vector2>();
    }

    public void OnDropBomb(InputValue value)
    {
        // call drop bomb function
        Debug.Log("Drop Bomb");
    }
}
