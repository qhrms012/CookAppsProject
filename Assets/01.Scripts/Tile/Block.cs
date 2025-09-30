using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public enum ColorType { Red, Orange, Green, Yellow, Purple, Pink }

    [Header("Block Info")]
    public ColorType color;           // Inspector���� ����
    public Vector2Int gridPos;        // ���� �� ��ġ (x,y)

    // �ʱ�ȭ �Լ�
    public void Init(ColorType c, Vector2Int pos)
    {
        color = c;
        gridPos = pos;
        name = $"Block_{color}_{pos.x}_{pos.y}";
    }
}
