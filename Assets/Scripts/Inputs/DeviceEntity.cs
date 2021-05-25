using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DeviceEntity : MonoBehaviour
{
    public static List<DeviceEntity> instancesList = new List<DeviceEntity>();

    public SO_Int ConnectedCount;
    public bool connected = true;
    public byte localPlayerIndex = 0;

    public PhysicalDevice inputs;

    private PlayerInput playerInput;
    private Gamepad myGamepad;

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        
        inputs = GetComponent<PhysicalDevice>();

        localPlayerIndex = (byte)instancesList.Count;

        name += " "+ localPlayerIndex.ToString();
        instancesList.Add(this);
    }

    public PlayerInput GetPhysicalInput()
    {
        return playerInput;
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

}
