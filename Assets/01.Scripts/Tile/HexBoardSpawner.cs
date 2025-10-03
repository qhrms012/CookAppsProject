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

    public Dictionary<Vector2Int, Block> blockDict = new Dictionary<Vector2Int, Block>();

    public static readonly Vector3Int[] cubeDirs = new Vector3Int[]
    {
        new Vector3Int( 1, -1,  0),   // ��
        new Vector3Int( 1,  0, -1),   // ��
        new Vector3Int( 0,  1, -1),   // ��
        new Vector3Int(-1,  1,  0),   // ��
        new Vector3Int(-1,  0,  1),   // ��
        new Vector3Int( 0, -1,  1)    // ��
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
                    Vector2Int offset = new Vector2Int(x, y);
                    SpawnSingleBlock(offset);
                }
            }
        }
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

    // === ��ǥ ��ȯ ===
    public Vector3Int OffsetToCube(Vector2Int offset)
    {
        int col = offset.x;
        int row = offset.y;

        int x = col - (row - (row & 1)) / 2; // Odd-R ����
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

    // === ��ġ �� ���� �� ���/���� ===
    public void ProcessMatches(List<List<Block>> matches)
    {
        

        // ����
        matchManager.ClearMatches(matches, blockDict);

        // ��� + ����
        StartCoroutine(DropAndRefill());
    }

    private IEnumerator DropAndRefill()
    {
        yield return new WaitForSeconds(0.25f);

        BoundsInt bounds = bgTilemap.cellBounds;

        // y �������� ������ �Ʒ��� Ž�� (�� ����)
        for (int y = bounds.yMin; y < bounds.yMax; y++)
        {
            for (int x = bounds.xMin; x < bounds.xMax; x++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                if (bgTilemap.HasTile(new Vector3Int(x, y, 0)) && !blockDict.ContainsKey(pos))
                {
                    // ���ʿ��� �� ã��
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

                    // ������ �ƹ��͵� �� ã���� �� �� ����
                    if (!blockDict.ContainsKey(pos))
                    {
                        // ������ ������ �ٱ�(xMax+1)�� �ӽ� ����
                        Vector3Int spawnCell = new Vector3Int(bounds.xMax + 1, y, 0);
                        Vector3 spawnWorld = bgTilemap.GetCellCenterWorld(spawnCell);

                        Block prefab = blockPrefabs[Random.Range(0, blockPrefabs.Length)];
                        Block newBlock = Instantiate(prefab, spawnWorld, Quaternion.identity);
                        newBlock.Init(newBlock.color, pos);
                        newBlock.spawner = this;

                        blockDict[pos] = newBlock;

                        // ���� ��ġ���� �ִϸ��̼� �̵�
                        Vector3 targetWorld = bgTilemap.GetCellCenterWorld(new Vector3Int(pos.x, pos.y, 0));
                        StartCoroutine(newBlock.MoveTo(targetWorld, 0.25f));
                    }

                }
            }
        }

        yield return new WaitForSeconds(0.3f);

        // ��� �� �ٽ� ��ġ �˻� (���� ó��)
        var newMatches = matchManager.FindMatches(blockDict);
        if (newMatches.Count > 0)
            ProcessMatches(newMatches);
    }
}
