using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI clownCount;
    public TextMeshProUGUI moveCount;

    public RectTransform clownUI;
    private void Start()
    {
        
    }

    public void MoveClownToGoal()
    {
        Vector3 GoalWorldPos = Camera.main.ScreenToWorldPoint(clownUI.position);
    }

}
