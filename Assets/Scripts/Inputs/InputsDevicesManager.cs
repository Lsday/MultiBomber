using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.EventSystems;

public class InputsDevicesManager : MonoBehaviour
{

    public bool allowKeyboardPlayer = true;

    public GameObject inputPrefab;
    public SO_Int ConnectedCount;

    PlayerInputManager inputManager;
    void Start()
    {

        inputManager = GetComponent<PlayerInputManager>();
        ConnectedCount.value = 0;

        InvokeRepeating("DetectNewPlayers", 0.1f, 0.5f);
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(0, 0, 50, 25), ConnectedCount.value.ToString());
    }

    public void OnPlayerJoined()
    {
        Debug.Log("OnPlayerJoined");
    }

    public void OnPlayerLeft()
    {
        Debug.Log("OnPlayerLeft");
    }

    // Get all unused devices and assign a new player input prefab to each
    private void DetectNewPlayers()
    {
        InputControlList<InputDevice> list = InputUser.GetUnpairedInputDevices();
        if (list.Count > 0)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].device is Gamepad || list[i].device is Joystick || (allowKeyboardPlayer && list[i].device is Keyboard))
                {
                    PlayerInput.Instantiate(inputPrefab, pairWithDevice: list[i]);
                    ConnectedCount.value++;
                }
            }
        }
    }
}
