using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public enum ColorType { Red, Orange, Green, Yellow, Purple, Pink }

    [Header("Block Info")]
    public ColorType color;
    public Vector2Int gridPos;   // ���� �� ��ġ

    private Vector3 dragOffset;
    private Vector3 originalPos;
    private Camera cam;

    public HexBoardSpawner spawner;

    private void OnEnable()
    {
        spawner = FindAnyObjectByType<HexBoardSpawner>();
    }

    private void Start()
    {
        cam = Camera.main;
    }

    // �巡�� ����
    private void OnMouseDown()
    {
        originalPos = transform.position;
        Vector3 mouseWorld = cam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0;
        dragOffset = transform.position - mouseWorld;
    }

    // �巡�� ��
    private void OnMouseDrag()
    {
        Vector3 mouseWorld = cam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0;
        transform.position = mouseWorld + dragOffset;
    }

    // �巡�� ����
    private void OnMouseUp()
    {
        Vector3 mouseWorld = cam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0;
        Vector3Int cell = spawner.bgTilemap.WorldToCell(mouseWorld);
        Vector2Int dropPos = new Vector2Int(cell.x, cell.y);

        // ���� �ڸ� �ƴϰ�, �̿� ���̸� ���� �õ�
        if (dropPos != gridPos &&
            spawner.blockDict.ContainsKey(dropPos) &&
            IsNeighbor(gridPos, dropPos))
        {
            Block other = spawner.blockDict[dropPos];
            TrySwap(other);
        }
        else
        {
            // �ƴϸ� ���� �ڸ��� ����
            StartCoroutine(MoveTo(originalPos, 0.2f));
        }
    }

    // ���� �õ�
    private void TrySwap(Block other)
    {
        Vector2Int posA = this.gridPos;
        Vector2Int posB = other.gridPos;

        // ��ųʸ� ����
        spawner.blockDict[posA] = other;
        spawner.blockDict[posB] = this;

        // �� ���� ��ǥ ��ȯ
        this.gridPos = posB;
        other.gridPos = posA;

        // �ð��� �ڸ� ��ȯ
        Vector3 worldA = spawner.bgTilemap.GetCellCenterWorld(new Vector3Int(posA.x, posA.y, 0));
        Vector3 worldB = spawner.bgTilemap.GetCellCenterWorld(new Vector3Int(posB.x, posB.y, 0));
        StartCoroutine(this.MoveTo(worldB, 0.2f));
        StartCoroutine(other.MoveTo(worldA, 0.2f));

        // ��ġ �˻�
        var matches = spawner.matchManager.FindMatches(spawner.blockDict);
        if (matches.Count == 0)
        {
            // ��ġ ������ �ǵ�����
            StartCoroutine(this.MoveTo(worldA, 0.2f));
            StartCoroutine(other.MoveTo(worldB, 0.2f));

            // ��ǥ �� ��ųʸ� ����
            this.gridPos = posA;
            other.gridPos = posB;
            spawner.blockDict[posA] = this;
            spawner.blockDict[posB] = other;
        }
    }

    // �̿� ���� (Cube ��ǥ ���� == cubeDirs �� �ϳ����� Ȯ��)
    private bool IsNeighbor(Vector2Int a, Vector2Int b)
    {
        Vector3Int cubeA = spawner.matchManager.OffsetToCube(a);
        Vector3Int cubeB = spawner.matchManager.OffsetToCube(b);
        Vector3Int delta = cubeB - cubeA;

        foreach (var dir in MatchManager.cubeDirs)
            if (delta == dir) return true;

        return false;
    }

    // �ʱ�ȭ
    public void Init(ColorType c, Vector2Int pos)
    {
        color = c;
        gridPos = pos;
        name = $"Block_{color}_{pos.x}_{pos.y}";
    }

    // �̵� ����
    public IEnumerator MoveTo(Vector3 targetPos, float duration = 0.25f)
    {
        Vector3 startPos = transform.position;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            transform.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        transform.position = targetPos;
    }
}
