using System;
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

    public static event Action OnBlockSwapped;

    private bool isMoving = false;

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
        if (isMoving) return;
        originalPos = transform.position;

        Vector3 mouseWorld = cam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0;
        dragOffset = transform.position - mouseWorld;
    }

    private void OnMouseDrag()
    {
        if (isMoving) return;

        Vector3 mouseWorld = cam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0;
        transform.position = mouseWorld + dragOffset;
    }

    private void OnMouseUp()
    {
        if (isMoving) return;

        Vector3 mouseWorld = cam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0;
        Vector3Int cell = spawner.bgTilemap.WorldToCell(mouseWorld);
        Vector2Int dropPos = new Vector2Int(cell.x, cell.y);

        if (dropPos != gridPos &&
            spawner.blockDict.ContainsKey(dropPos) &&
            IsNeighbor(gridPos, dropPos))
        {
            Block other = spawner.blockDict[dropPos];
            StartCoroutine(SwapAndCheck(other));
        }
        else
        {
            StartCoroutine(MoveTo(originalPos, 0.2f));
        }
    }

    private IEnumerator SwapAndCheck(Block other)
    {
        if (isMoving) yield break;
        isMoving = true;

        Vector2Int posA = this.gridPos;
        Vector2Int posB = other.gridPos;

        Vector3 worldA = spawner.bgTilemap.GetCellCenterWorld(new Vector3Int(posA.x, posA.y, 0));
        Vector3 worldB = spawner.bgTilemap.GetCellCenterWorld(new Vector3Int(posB.x, posB.y, 0));

        yield return StartCoroutine(this.MoveTo(worldB, 0.25f));
        yield return StartCoroutine(other.MoveTo(worldA, 0.25f));

        spawner.blockDict[posA] = other;
        spawner.blockDict[posB] = this;
        this.gridPos = posB;
        other.gridPos = posA;

        var matches = spawner.matchManager.FindMatches(spawner.blockDict);

        if (matches.Count == 0)
        {
            yield return StartCoroutine(this.MoveTo(worldA, 0.25f));
            yield return StartCoroutine(other.MoveTo(worldB, 0.25f));

            spawner.blockDict[posA] = this;
            spawner.blockDict[posB] = other;
            this.gridPos = posA;
            other.gridPos = posB;
        }
        else
        {
            spawner.ProcessMatches(matches);
        }

        isMoving = false;
        OnBlockSwapped?.Invoke();
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

