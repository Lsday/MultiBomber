using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.EventSystems;

public class InputsDevicesManager : MonoBehaviour
{
    public static InputsDevicesManager instance;

    public bool allowKeyboardPlayer = true;

    public GameObject physicalDevicePrefab;
    public GameObject virtualDevicePrefab;

    public SO_Int physicalDevicesCount;
    public SO_Int localPlayersCount;
    public SO_Int totalPlayersCount;

    PlayerInputManager inputManager;
    void Start()
    {
        transform.position = Vector3.zero;

        inputManager = GetComponent<PlayerInputManager>();

        InvokeRepeating("DetectNewPlayers", 0.1f, 0.5f); //TODO : AJOUTER TIMER CLASS
    }

    private void OnGUI()
    {
        if (totalPlayersCount.value < 10)
        {
            if (GUI.Button(new Rect(150, 5, 100, 25), "Add BOT"))
            {
                AddBot();
            }
        }
    }

    public void OnPlayerJoined()
    {
        //Debug.Log("OnPlayerJoined");
    }

    public void OnPlayerLeft()
    {
        //Debug.Log("OnPlayerLeft");
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
                    AddHuman(list[i]);
                }
            }
        }
    }

    private void AddHuman(InputDevice device)
    {
        PlayerInput pi = PlayerInput.Instantiate(physicalDevicePrefab, pairWithDevice: device);
        pi.transform.parent = transform;

        if (PlayersHub.instance != null)
        {
            PlayersHub.instance.AddPlayer((byte)localPlayersCount.value);
        }
        physicalDevicesCount.value++;

        localPlayersCount.value++;
        totalPlayersCount.value++;
    }

    private void AddBot()
    {
        Instantiate(virtualDevicePrefab, transform);

        if (PlayersHub.instance != null)
        {
            PlayersHub.instance.AddPlayer((byte)localPlayersCount.value);
        }

        localPlayersCount.value++;
        totalPlayersCount.value++;
    }
}
