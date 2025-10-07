using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class HexBoardSpawner : MonoBehaviour
{
    [Header("Tilemap")]
    public Tilemap bgTilemap;

    [Header("Blocks")]
    public Block[] blockPrefabs;

    [Header("Manager")]
    public MatchManager matchManager;

    [Header("JackBox")]
    public JackBoxManager jackBoxManager;

    public Dictionary<Vector2Int, Block> blockDict = new Dictionary<Vector2Int, Block>();

    public static readonly Vector3Int[] cubeDirs = new Vector3Int[]
    {
        new Vector3Int( 1, -1,  0),   // ¡æ
        new Vector3Int( 1,  0, -1),   // ¢Ö
        new Vector3Int( 0,  1, -1),   // ¢Ø
        new Vector3Int(-1,  1,  0),   // ¡ç
        new Vector3Int(-1,  0,  1),   // ¢×
        new Vector3Int( 0, -1,  1)    // ¢Ù
    };

    private void Start()
    {
        SpawnBlocks();
    }

    private void SpawnBlocks()
    {
        BoundsInt bounds = bgTilemap.cellBounds;

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int cellPos = new Vector3Int(x, y, 0);

                if (bgTilemap.HasTile(cellPos))
                {
                    Vector2Int gridPos = new Vector2Int(x, y);

                    Block prefab = GetSafeRandomBlock(gridPos);
                    Vector3 worldPos = bgTilemap.GetCellCenterWorld(cellPos);

                    Block b = Instantiate(prefab, worldPos, Quaternion.identity);
                    b.Init(b.color, gridPos);
                    b.spawner = this;

                    blockDict[gridPos] = b;
                }
            }
        }
    }
    private Block GetSafeRandomBlock(Vector2Int pos)
    {
        int tries = 0;
        Block prefab;

        do
        {
            prefab = blockPrefabs[Random.Range(0, blockPrefabs.Length)];
            tries++;
        }
        while (WouldCauseMatch(prefab.color, pos) && tries < 20);

        return prefab;
    }
    private bool WouldCauseMatch(Block.ColorType color, Vector2Int pos)
    {
        Vector3Int cubePos = OffsetToCube(pos);

        foreach (var dir in cubeDirs)
        {
            Vector3Int n1 = cubePos + dir;
            Vector3Int n2 = cubePos + dir * 2;

            Vector2Int o1 = CubeToOffset(n1);
            Vector2Int o2 = CubeToOffset(n2);

            if (blockDict.TryGetValue(o1, out Block b1) &&
                blockDict.TryGetValue(o2, out Block b2))
            {
                if (b1.color == color && b2.color == color)
                    return true;
            }
        }

        return false;
    }

    public Block SpawnSingleBlock(Vector2Int offset)
    {
        Vector3 worldPos = bgTilemap.GetCellCenterWorld(new Vector3Int(offset.x, offset.y, 0));
        Block prefab = blockPrefabs[Random.Range(0, blockPrefabs.Length)];
        Block block = Instantiate(prefab, worldPos, Quaternion.identity);

        block.Init(block.color, offset);
        block.spawner = this;

        blockDict[offset] = block;
        return block;
    }

    public Vector3Int OffsetToCube(Vector2Int offset)
    {
        int col = offset.x;
        int row = offset.y;

        int x = col - (row - (row & 1)) / 2;
        int z = row;
        int y = -x - z;

        return new Vector3Int(x, y, z);
    }

    public Vector2Int CubeToOffset(Vector3Int cube)
    {
        int col = cube.x + (cube.z - (cube.z & 1)) / 2;
        int row = cube.z;
        return new Vector2Int(col, row);
    }


    public void ProcessMatches(List<List<Block>> matches)
    {
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.Pop);

        matchManager.ClearMatches(matches, blockDict);

        NotifyJackBoxNearby(matches);

        StartCoroutine(DropAndRefill());
    }

    private void NotifyJackBoxNearby(List<List<Block>> matches)
    {
        foreach (var run in matches)
        {
            foreach (var b in run)
            {
                Vector3 blockPos = bgTilemap.GetCellCenterWorld(new Vector3Int(b.gridPos.x, b.gridPos.y, 0));

                foreach (var jack in jackBoxManager.jackBoxes)
                {
                    float dist = Vector3.Distance(blockPos, jack.transform.position);
                    if(dist < 1.1f)
                    {
                        jack.OnNearbyMatch();
                    }
                }
            }
        }
    }
    private IEnumerator DropAndRefill()
    {
        yield return new WaitForSeconds(0.25f);

        BoundsInt bounds = bgTilemap.cellBounds;

        for (int y = bounds.yMin; y < bounds.yMax; y++)
        {
            for (int x = bounds.xMin; x < bounds.xMax; x++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                if (bgTilemap.HasTile(new Vector3Int(x, y, 0)) && !blockDict.ContainsKey(pos))
                {
                    for (int nx = x + 1; nx < bounds.xMax; nx++)
                    {
                        Vector2Int abovePos = new Vector2Int(nx, y);
                        if (blockDict.ContainsKey(abovePos))
                        {
                            Block aboveBlock = blockDict[abovePos];
                            blockDict.Remove(abovePos);

                            aboveBlock.gridPos = pos;
                            blockDict[pos] = aboveBlock;

                            Vector3 worldPos = bgTilemap.GetCellCenterWorld(new Vector3Int(pos.x, pos.y, 0));
                            StartCoroutine(aboveBlock.MoveTo(worldPos, 0.25f));
                            break;
                        }
                    }

                    if (!blockDict.ContainsKey(pos))
                    {
                        Vector3Int spawnCell = new Vector3Int(bounds.xMax + 1, y, 0);
                        Vector3 spawnWorld = bgTilemap.GetCellCenterWorld(spawnCell);

                        Block prefab = blockPrefabs[Random.Range(0, blockPrefabs.Length)];
                        Block newBlock = Instantiate(prefab, spawnWorld, Quaternion.identity);
                        newBlock.Init(newBlock.color, pos);
                        newBlock.spawner = this;

                        blockDict[pos] = newBlock;

                        Vector3 targetWorld = bgTilemap.GetCellCenterWorld(new Vector3Int(pos.x, pos.y, 0));
                        StartCoroutine(newBlock.MoveTo(targetWorld, 0.25f));
                    }

                }
            }
        }

        yield return new WaitForSeconds(0.3f);

        var newMatches = matchManager.FindMatches(blockDict);
        if (newMatches.Count > 0)
            ProcessMatches(newMatches);
    }
}
