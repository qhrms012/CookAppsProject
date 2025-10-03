using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Block : MonoBehaviour
{
    public enum ColorType { Red, Orange, Green, Yellow, Purple, Pink }
    public ColorType color;
    public Vector2Int gridPos;

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

    private void OnMouseDown()
    {
        originalPos = transform.position;
        Vector3 mouseWorld = cam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0;
        dragOffset = transform.position - mouseWorld;
    }

    private void OnMouseDrag()
    {
        Vector3 mouseWorld = cam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0;
        transform.position = mouseWorld + dragOffset;
    }

    private void OnMouseUp()
    {
        Vector3 mouseWorld = cam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0;
        Vector3Int cell = spawner.bgTilemap.WorldToCell(mouseWorld);
        Vector2Int dropPos = new Vector2Int(cell.x, cell.y);

        // 이웃 검사 후 스왑 시도
        if (dropPos != gridPos &&
            spawner.blockDict.ContainsKey(dropPos) &&
            IsNeighbor(gridPos, dropPos))
        {
            Block other = spawner.blockDict[dropPos];
            TrySwap(other);
        }
        else
        {
            StartCoroutine(MoveTo(originalPos, 0.2f));
        }
    }

    private void TrySwap(Block other)
    {
        Vector2Int posA = this.gridPos;
        Vector2Int posB = other.gridPos;

        // 딕셔너리 먼저 갱신 (위치 확정)
        spawner.blockDict[posA] = other;
        spawner.blockDict[posB] = this;

        this.gridPos = posB;
        other.gridPos = posA;

        // 자리 교환 연출
        Vector3 worldA = spawner.bgTilemap.GetCellCenterWorld(new Vector3Int(posA.x, posA.y, 0));
        Vector3 worldB = spawner.bgTilemap.GetCellCenterWorld(new Vector3Int(posB.x, posB.y, 0));
        StartCoroutine(SwapAndCheck(this, other, worldA, worldB));
    }

    private IEnumerator SwapAndCheck(Block a, Block b, Vector3 worldA, Vector3 worldB)
    {
        // 애니메이션 완료 대기
        yield return a.StartCoroutine(a.MoveTo(worldB, 0.2f));
        yield return b.StartCoroutine(b.MoveTo(worldA, 0.2f));

        var matches = spawner.matchManager.FindMatches(spawner.blockDict);

        if (matches.Count == 0)
        {
            // 매치 없으면 다시 원래 자리로 되돌리기
            spawner.blockDict[a.gridPos] = b;
            spawner.blockDict[b.gridPos] = a;

            Vector2Int oldA = b.gridPos;
            Vector2Int oldB = a.gridPos;

            a.gridPos = oldB;
            b.gridPos = oldA;

            yield return a.StartCoroutine(a.MoveTo(worldA, 0.2f));
            yield return b.StartCoroutine(b.MoveTo(worldB, 0.2f));
        }
        else
        {
            // 매치 있으면 프로세스 진행
            spawner.ProcessMatches(matches);
        }
    }


    private bool IsNeighbor(Vector2Int a, Vector2Int b)
    {
        Vector3Int cubeA = spawner.OffsetToCube(a);
        Vector3Int cubeB = spawner.OffsetToCube(b);
        Vector3Int delta = cubeB - cubeA;

        foreach (var dir in HexBoardSpawner.cubeDirs)
            if (delta == dir) return true;

        return false;
    }

    public void Init(ColorType c, Vector2Int pos)
    {
        color = c;
        gridPos = pos;
        name = $"Block_{color}_{pos.x}_{pos.y}";
    }

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
