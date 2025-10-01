using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchManager : MonoBehaviour
{
    [Header("Reference")]
    public HexBoardSpawner spawner;

    // Cube 좌표 6방향 (직선)
    private static readonly Vector3Int[] cubeDirs = new Vector3Int[]
    {
        new Vector3Int(1, -1, 0),   // →
        new Vector3Int(1, 0, -1),   // ↗
        new Vector3Int(0, 1, -1),   // ↖
        new Vector3Int(-1, 1, 0),   // ←
        new Vector3Int(-1, 0, 1),   // ↙
        new Vector3Int(0, -1, 1)    // ↘
    };

    private void Start()
    {
        StartCoroutine(AutoMatchLoop());
    }

    IEnumerator AutoMatchLoop()
    {
        while (true)
        {
            // 매치 찾기
            var matches = FindMatches(spawner.blockDict);

            if (matches.Count > 0)
            {
                Debug.Log($"Match found! {matches.Count} group(s)");
                ClearMatches(matches, spawner.blockDict);

                yield return new WaitForSeconds(0.3f);

                
                spawner.DropAndRefill();

                // 리필 후 다시 매치 검사 (루프 계속)
            }
            else
            {
                // 매치가 없으면 → 한 프레임 대기 후 다시 검사
                yield return null;
            }
        }
    }

    Vector3Int OffsetToCube(Vector2Int offset)
    {
        int col = offset.x;
        int row = offset.y;

        int x = col - (row - (row & 1)) / 2;   // 홀수 행(row)에서 보정
        int z = row;
        int y = -x - z;

        return new Vector3Int(x, y, z);
    }

    Vector2Int CubeToOffset(Vector3Int cube)
    {
        int col = cube.x + (cube.z - (cube.z & 1)) / 2;
        int row = cube.z;
        return new Vector2Int(col, row);
    }

    public List<List<Block>> FindMatches(Dictionary<Vector2Int, Block> dict)
    {
        List<List<Block>> matches = new List<List<Block>>();
        HashSet<Block> visited = new HashSet<Block>();

        foreach (var kvp in dict)
        {
            Vector2Int pos = kvp.Key;
            Block block = kvp.Value;

            if (visited.Contains(block))
                continue;

            Vector3Int cubePos = OffsetToCube(pos);

            // 3쌍의 직선 (0↔3, 1↔4, 2↔5)
            for (int d = 0; d < 3; d++)
            {
                List<Block> run = new List<Block> { block };

                // 정방향 탐색
                Vector3Int cur = cubePos;
                while (true)
                {
                    cur += cubeDirs[d];
                    Vector2Int off = CubeToOffset(cur);

                    if (dict.TryGetValue(off, out Block next) && next.color == block.color)
                    {
                        run.Add(next);
                    }
                    else break;
                }

                // 역방향 탐색
                cur = cubePos;
                while (true)
                {
                    cur += cubeDirs[d + 3];
                    Vector2Int off = CubeToOffset(cur);

                    if (dict.TryGetValue(off, out Block prev) && prev.color == block.color)
                    {
                        run.Add(prev);
                    }
                    else break;
                }

                if (run.Count >= 3)
                {
                    matches.Add(run);

                    foreach (var b in run)
                        visited.Add(b);

                    string coords = "";
                    foreach (var b in run) coords += $"({b.gridPos.x},{b.gridPos.y}) ";
                    Debug.Log($"Match found of color {block.color}: {coords}");
                }
            }
        }

        return matches;
    }

    public void ClearMatches(List<List<Block>> matches, Dictionary<Vector2Int, Block> dict)
    {
        HashSet<Block> toRemove = new HashSet<Block>();

        foreach (var run in matches)
        {
            foreach (var b in run)
            {
                toRemove.Add(b);
            }
        }

        foreach (var b in toRemove)
        {
            if (dict.ContainsKey(b.gridPos))
            {
                dict.Remove(b.gridPos);
                Destroy(b.gameObject);
            }
        }
    }
}


