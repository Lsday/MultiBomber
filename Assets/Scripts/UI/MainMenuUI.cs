using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using Mirror;

public class MainMenuUI : MonoBehaviour
{
    public GameObject mainPanel;

    public GameObject gameModeButtonsPanel;
    public Button localButton, multiPlayerButton;

    public GameObject localButtonsPanel;
    public Button startButton, localBackButton;

    public GameObject mutliplayerButtonsPanel;
    public Button hostButton, joinButton, multiBackButton;


    private void Start()
    {
        // GAME MODE SELECTION
        localButton.onClick.AddListener(OnLocalButtonPressed);
        multiPlayerButton.onClick.AddListener(OnMultiButtonPressed);

        //LOCAL MODE
        //TODO : startbutton
        localBackButton.onClick.AddListener(OnlocalBackButtonPressed);

        //MULTIPLAYER MODE

        hostButton.onClick.AddListener(OnHostButtonPressed);
        joinButton.onClick.AddListener(OnJoinButtonPressed);
        multiBackButton.onClick.AddListener(OnMutliplayerBackButtonPressed);

    }

    #region Navigation

    public void DisableMainPanel() => mainPanel.SetActive(false);
    public void DisableGameModeButtonsPanel() => gameModeButtonsPanel.SetActive(false);
    public void DisableLocalButtonsPanel() => localButtonsPanel.SetActive(false);
    public void DisableMutliplayerButtonsPanel() => mutliplayerButtonsPanel.SetActive(false);

    public void EnableMainPanel() => mainPanel.SetActive(true);
    public void EnableGameModeButtonsPanel() => gameModeButtonsPanel.SetActive(true);
    public void EnableLocalButtonsPanel() => localButtonsPanel.SetActive(true);
    public void EnableMutliplayerButtonsPanel() => mutliplayerButtonsPanel.SetActive(true);

    private void OnLocalButtonPressed()
    {
        DisableGameModeButtonsPanel();
        EnableLocalButtonsPanel();
    }
    private void OnMultiButtonPressed()
    {
        DisableGameModeButtonsPanel();
        EnableMutliplayerButtonsPanel();
    }
    private void OnlocalBackButtonPressed()
    {
        DisableLocalButtonsPanel();
        EnableGameModeButtonsPanel();
    }
    private void OnMutliplayerBackButtonPressed()
    {
        DisableMutliplayerButtonsPanel();
        EnableGameModeButtonsPanel();
    }

    #endregion
    private void OnHostButtonPressed()
    {
        NetworkManager.singleton.StartHost();
        DisableMainPanel();
    }
    private void OnJoinButtonPressed()
    {
        NetworkManager.singleton.StartClient();
        DisableMainPanel();
    }
}
