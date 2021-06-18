using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    
    public double gameLength = 45;
    public double gameAlmostEnd = 30;
    public GameTimerUI gameTimerUI;
    public double startTimer;
    public double gameTimer;

    bool isGameStarted;
    private bool isGameAlmostEnded;
    private bool isGameEnded;

    public event GameState onGameAlmostEnded;
    public event GameState onGameEnded;

    int _minutes;
    int _seconds;

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
            UpdateTimer((float)(gameLength - gameTimer));

            if (gameTimer >= (gameLength - gameAlmostEnd) && isGameAlmostEnded == false)
            {
                isGameAlmostEnded = true;
                onGameAlmostEnded?.Invoke();
                Debug.Log("Game almost ended");
            }

            if (gameTimer >= gameLength && isGameEnded == false)
            {
                isGameEnded = true;
                onGameEnded?.Invoke();
                isGameStarted = false;
                Debug.Log("Game ended");
            }
        }
        else
            gameTimerUI.SetTimer("-");
    }

    private void UpdateTimer(float value)
    {
        int minutes = Mathf.FloorToInt(value / 60);
        int seconds = Mathf.FloorToInt(value - (minutes * 60));
        int milliseconds = Mathf.FloorToInt((value % 1) * 1000);

        if (_minutes != minutes || _seconds != seconds || true)
        {
            gameTimerUI.SetTimer(string.Format("{0}:{1}.{2}", minutes.ToString("00"), seconds.ToString("00"), milliseconds.ToString("000")));
        }

        _minutes = minutes;
        _seconds = seconds;
    }
}
