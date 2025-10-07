using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MatchManager : MonoBehaviour
{
    [Header("Reference")]
    public HexBoardSpawner spawner;

    public List<List<Block>> FindMatches(Dictionary<Vector2Int, Block> dict)
    {
        List<List<Block>> matches = new List<List<Block>>();
        HashSet<Block> visited = new HashSet<Block>();

        foreach (var kvp in dict)
        {
            Block block = kvp.Value;
            if (visited.Contains(block)) continue;

            Vector3Int cubePos = spawner.OffsetToCube(block.gridPos);

            for (int d = 0; d < 3; d++)
            {
                Vector3Int dir = HexBoardSpawner.cubeDirs[d];
                Vector3Int opp = HexBoardSpawner.cubeDirs[d + 3];

                List<Block> run = new List<Block> { block };

                Vector3Int cur = cubePos;
                while (true)
                {
                    cur += dir;
                    Vector2Int offset = spawner.CubeToOffset(cur);
                    if (dict.TryGetValue(offset, out Block next) && next.color == block.color)
                        run.Add(next);
                    else break;
                }

                // ¿ª¹æÇâ
                cur = cubePos;
                while (true)
                {
                    cur += opp;
                    Vector2Int offset = spawner.CubeToOffset(cur);
                    if (dict.TryGetValue(offset, out Block next) && next.color == block.color)
                        run.Add(next);
                    else break;
                }

                if (run.Count >= 3)
                {
                    matches.Add(run);
                    foreach (var b in run) visited.Add(b);
                }
            }
        }
        return matches;
    }

    public void ClearMatches(List<List<Block>> matches, Dictionary<Vector2Int, Block> dict)
    {
        foreach (var run in matches)
        {
            foreach (var b in run)
            {
                if (dict.ContainsKey(b.gridPos))
                {
                    dict.Remove(b.gridPos);
                    Destroy(b.gameObject);
                }
            }
        }
    }
}


