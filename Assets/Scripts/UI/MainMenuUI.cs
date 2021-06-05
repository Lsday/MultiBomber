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

    public GameObject  mapOptionsPanel;

    MapOptionsUI mapOptionsUI;

    private void Start()
    {
        NetworkClient.RegisterHandler<GameStartedMessage>(GameStart);

        // GAME MODE SELECTION
        localButton.onClick.AddListener(OnLocalButtonPressed);
        multiPlayerButton.onClick.AddListener(OnMultiButtonPressed);

        //LOCAL MODE
        startButton.onClick.AddListener(EnableMapOptionsPanel);
        localBackButton.onClick.AddListener(OnlocalBackButtonPressed);

        //MULTIPLAYER MODE

        hostButton.onClick.AddListener(OnHostButtonPressed);
        joinButton.onClick.AddListener(OnJoinButtonPressed);
        multiBackButton.onClick.AddListener(OnMutliplayerBackButtonPressed);

    }

    private void GameStart(GameStartedMessage arg2)
    {
         mainPanel.SetActive(false);
    }

    private void EnableMapOptionsPanel()
    {
        mapOptionsPanel.SetActive(true);
        mapOptionsUI = mapOptionsPanel.GetComponent<MapOptionsUI>();
        mapOptionsUI.Init();



    }

    #region Navigation

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
        DisableMutliplayerButtonsPanel();
        EnableMapOptionsPanel();
    }
    private void OnJoinButtonPressed()
    {
        NetworkManager.singleton.StartClient();
        DisableMutliplayerButtonsPanel();
       
    }
}
