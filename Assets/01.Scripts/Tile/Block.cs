using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public enum ColorType { Red, Orange, Green, Yellow, Purple, Pink }

    [Header("Block Info")]
    public ColorType color;           // Inspector에서 지정
    public Vector2Int gridPos;        // 보드 상 위치 (x,y)

    // 초기화 함수
    public void Init(ColorType c, Vector2Int pos)
    {
        color = c;
        gridPos = pos;
        name = $"Block_{color}_{pos.x}_{pos.y}";
    }
}
