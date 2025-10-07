using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private GameOverUI gameOverUI;

    public static event Action<bool> onGameOver;
    public bool isGameOver = false;


    private void Start()
    {
        
    }


    public void TriggerGameOver(bool isWin)
    {        
        onGameOver?.Invoke(isWin);
        Time.timeScale = 0;
    }
}
