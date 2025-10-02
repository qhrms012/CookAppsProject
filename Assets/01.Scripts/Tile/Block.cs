using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public enum ColorType { Red, Orange, Green, Yellow, Purple, Pink }

    [Header("Block Info")]
    public ColorType color;
    public Vector2Int gridPos;   // 보드 상 위치

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

    // 드래그 시작
    private void OnMouseDown()
    {
        originalPos = transform.position;
        Vector3 mouseWorld = cam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0;
        dragOffset = transform.position - mouseWorld;
    }

    // 드래그 중
    private void OnMouseDrag()
    {
        Vector3 mouseWorld = cam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0;
        transform.position = mouseWorld + dragOffset;
    }

    // 드래그 종료
    private void OnMouseUp()
    {
        Vector3 mouseWorld = cam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0;
        Vector3Int cell = spawner.bgTilemap.WorldToCell(mouseWorld);
        Vector2Int dropPos = new Vector2Int(cell.x, cell.y);

        // 같은 자리 아니고, 이웃 블럭이면 스왑 시도
        if (dropPos != gridPos &&
            spawner.blockDict.ContainsKey(dropPos) &&
            IsNeighbor(gridPos, dropPos))
        {
            Block other = spawner.blockDict[dropPos];
            TrySwap(other);
        }
        else
        {
            // 아니면 원래 자리로 복귀
            StartCoroutine(MoveTo(originalPos, 0.2f));
        }
    }

    // 스왑 시도
    private void TrySwap(Block other)
    {
        Vector2Int posA = this.gridPos;
        Vector2Int posB = other.gridPos;

        // 딕셔너리 갱신
        spawner.blockDict[posA] = other;
        spawner.blockDict[posB] = this;

        // 블럭 내부 좌표 교환
        this.gridPos = posB;
        other.gridPos = posA;

        // 시각적 자리 교환
        Vector3 worldA = spawner.bgTilemap.GetCellCenterWorld(new Vector3Int(posA.x, posA.y, 0));
        Vector3 worldB = spawner.bgTilemap.GetCellCenterWorld(new Vector3Int(posB.x, posB.y, 0));
        StartCoroutine(this.MoveTo(worldB, 0.2f));
        StartCoroutine(other.MoveTo(worldA, 0.2f));

        // 매치 검사
        var matches = spawner.matchManager.FindMatches(spawner.blockDict);
        if (matches.Count == 0)
        {
            // 매치 없으면 되돌리기
            StartCoroutine(this.MoveTo(worldA, 0.2f));
            StartCoroutine(other.MoveTo(worldB, 0.2f));

            // 좌표 및 딕셔너리 원복
            this.gridPos = posA;
            other.gridPos = posB;
            spawner.blockDict[posA] = this;
            spawner.blockDict[posB] = other;
        }
    }

    // 이웃 판정 (Cube 좌표 차이 == cubeDirs 중 하나인지 확인)
    private bool IsNeighbor(Vector2Int a, Vector2Int b)
    {
        Vector3Int cubeA = spawner.matchManager.OffsetToCube(a);
        Vector3Int cubeB = spawner.matchManager.OffsetToCube(b);
        Vector3Int delta = cubeB - cubeA;

        foreach (var dir in MatchManager.cubeDirs)
            if (delta == dir) return true;

        return false;
    }

    // 초기화
    public void Init(ColorType c, Vector2Int pos)
    {
        color = c;
        gridPos = pos;
        name = $"Block_{color}_{pos.x}_{pos.y}";
    }

    // 이동 연출
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
