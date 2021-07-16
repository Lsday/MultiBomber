using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public struct GameStartedMessage : NetworkMessage { };

public delegate void GameState();

public class GameManager : NetworkBehaviour
{
    public event GameState onGameStarted;

    public static GameManager instance;


    private void Awake()
    {
        instance = this;
    }

    [ClientRpc]
    public void RpcStartGame()
    {
        onGameStarted?.Invoke();
    }


}
