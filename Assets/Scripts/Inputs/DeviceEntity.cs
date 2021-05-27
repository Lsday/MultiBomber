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

    public PhysicalDevice inputs; // pointer to the class used to receive inputs for this player : human (keyboard/gamepad) or a bot script

    private PlayerInput playerInput; // pointer to the InputSystem class referencing the keyboard or gamepad used to control this player (only is case of human player)
    private Gamepad myGamepad;

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        
        inputs = GetComponent<PhysicalDevice>();

        localPlayerIndex = (byte)instancesList.Count;

        name += " " + localPlayerIndex.ToString();
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

    // TODO : not sure this function is really necessary
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
        // to force refresh on anything linked to the connected players count scriptable object
        ConnectedCount.Refresh();
        DetectGamepad();
        Debug.Log("control changed " + name);
    }

}
