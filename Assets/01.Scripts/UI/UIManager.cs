using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    public TextMeshProUGUI clownCount;
    public TextMeshProUGUI moveCount;


    private int currentMoveCount = 10;
    private int currentClownCount = 10;
    private void Start()
    {
        moveCount.text = currentMoveCount.ToString();
        clownCount.text = currentClownCount.ToString();
    }
    private void OnEnable()
    {
        JackBox.onClownSpawned += UpdateClownCount;
        Block.OnBlockSwapped += UpdateMoveCount;
    }

    private void OnDisable()
    {
        JackBox.onClownSpawned -= UpdateClownCount;
        Block.OnBlockSwapped -= UpdateMoveCount;
    }

    private void UpdateClownCount(int value)
    {
        
        currentClownCount -= value;
        if(currentClownCount <= 0)
            GameManager.Instance.TriggerGameOver(true);

        clownCount.text = currentClownCount.ToString();
    }

    private void UpdateMoveCount()
    {
        if(currentMoveCount <= 0)
        {
            GameManager.Instance.TriggerGameOver(false);
        }

        currentMoveCount--;
        moveCount.text = currentMoveCount.ToString() ;
    }

}
