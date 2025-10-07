using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private GameOverUI gameOverUI;

    public static event Action<bool> onGameOver;
    public bool isGameOver = false;
    private bool isGamePause = false;

    private void Awake()
    {
        Time.timeScale = 1f;
        AudioManager.Instance.PlayBgm(true);
    }
    private void Start()
    {
        
    }

    public void PauseGame(bool isGamePause)
    {
        Time.timeScale = isGamePause ? 0f : 1f;
        AudioManager.Instance.EffectBgm(isGamePause);
    }
    public void TriggerGameOver(bool isWin)
    {        
        onGameOver?.Invoke(isWin);
    }

    public void GameReStart()
    {
        SceneManager.LoadScene("MainScene");
    }
}
