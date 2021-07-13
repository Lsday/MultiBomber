using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.EventSystems;

/// <summary>
/// This class manage all the inputs devices connected to the computer
/// a "physical" device is a keyboard or a gamepad used by a human player
/// a "virtual" device is a bot
/// </summary>
public class InputsDevicesManager : MonoBehaviour
{
    public static InputsDevicesManager instance;

    PlayerInputManager inputManager;


    /// <summary>
    /// If disabled, humans can only use gamepads to play
    /// </summary>
    public bool allowKeyboardPlayer = true;

    public GameObject physicalDevicePrefab;
    public GameObject virtualDevicePrefab;

    /// <summary>
    /// Number of physicale devices currently connected to the computer
    /// </summary>
    public SO_Int physicalDevicesCount;

    /// <summary>
    /// Number of local players actives on this computer (humans + bots)
    /// </summary>
    public SO_Int localPlayersCount;

    /// <summary>
    /// Total number of players (local + remote)
    /// </summary>
    public SO_Int totalPlayersCount;

    
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

    /// <summary>
    /// Get all unused devices and assign a new player input prefab to each
    /// </summary>
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

        //TODO : déplacer ces additions dans physicalDevicePrefab, ou trouver une manière plus propre de faire ce comptage
        physicalDevicesCount.value++;

        localPlayersCount.value++;
        totalPlayersCount.value++;
    }

    private void AddBot()
    {
        Debug.Log("A");
        Instantiate(virtualDevicePrefab, transform);

        Debug.Log("B");
        if (PlayersHub.instance != null)
        {
            Debug.Log("C");
            PlayersHub.instance.AddPlayer((byte)localPlayersCount.value);
        }
        Debug.Log("D");
        //TODO : déplacer ces additions dans physicalDevicePrefab, ou trouver une manière plus propre de faire ce comptage
        localPlayersCount.value++;
        totalPlayersCount.value++;
    }
}
