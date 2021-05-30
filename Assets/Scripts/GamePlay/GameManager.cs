using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public struct GameStartedMessage : NetworkMessage { };

public delegate void GameState();

public class GameManager : MonoBehaviour
{
    public event GameState onGameStarted;

    public static GameManager instance;

    public LevelBuilder levelBuilder;
    public PoolingSystem poolingSystem;


    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        NetworkClient.RegisterHandler<GameStartedMessage>(GameStartedCallBack);

        levelBuilder = GetComponent<LevelBuilder>();
        poolingSystem = GetComponent<PoolingSystem>();
       
    }



    private void GameStartedCallBack( GameStartedMessage arg2)
    {
        onGameStarted?.Invoke();
    }
}
