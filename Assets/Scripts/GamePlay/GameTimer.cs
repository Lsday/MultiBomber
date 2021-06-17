using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    
    public double gameLenght = 45;
    public double gameAlmostEnd = 30;
    public GameTimerUI gameTimerUI;
    public double startTimer;
    public double gameTimer;

    bool isGameStarted;
    private bool isGameAlmostEnded;
    private bool isGameEnded;

    public event GameState onGameAlmostEnded;
    public event GameState onGameEnded;

    void Start()
    {
        gameTimerUI = FindObjectOfType<GameTimerUI>();
        GameManager.instance.onGameStarted += StartGame;
    }

    void StartGame()
    {
        startTimer = NetworkTime.time;
        isGameStarted = true;
    }

    private void Update()
    {
        if (isGameStarted)
        {
            
            gameTimer = NetworkTime.time - startTimer - NetworkTime.offset;
            gameTimerUI.SetTimer((gameLenght - gameTimer).ToString());

            if (gameTimer >= (gameLenght - gameAlmostEnd) && isGameAlmostEnded == false)
            {
                isGameAlmostEnded = true;
                onGameAlmostEnded?.Invoke();
                Debug.Log("Game almost ended");
            }

            if (gameTimer >= gameLenght && isGameEnded == false)
            {
                isGameEnded = true;
                onGameEnded?.Invoke();
                isGameStarted = false;
                Debug.Log("Game ended");
            }
        }
        else
            gameTimerUI.SetTimer("0");

    }
}
