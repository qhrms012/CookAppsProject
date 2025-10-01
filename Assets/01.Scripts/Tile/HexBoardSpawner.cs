using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class HexBoardSpawner : MonoBehaviour
{
    [Header("Tilemap")]
    public Tilemap bgTilemap;           // ��Ʈ ������� ĥ�ص� ��� Ÿ�ϸ�
    

    [Header("Blocks")]
    public Block[] blockPrefabs;        // �� ������(����)

    public MatchManager matchManager;
    

    public Dictionary<Vector2Int, Block> blockDict = new Dictionary<Vector2Int, Block>();

    void Start()
    {
        SpawnBlocks();
    }

    void SpawnBlocks()
    {
        // Tilemap�� BoundsInt ���ϱ�
        BoundsInt bounds = bgTilemap.cellBounds;

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int cellPos = new Vector3Int(x, y, 0);

                
                if (bgTilemap.HasTile(cellPos))
                {
                    
                    Vector3 worldPos = bgTilemap.GetCellCenterWorld(cellPos);

                    
                    Block prefab = blockPrefabs[Random.Range(0, blockPrefabs.Length)];
                    Block b = Instantiate(prefab, worldPos, Quaternion.identity);
                    Vector2Int pos = new Vector2Int(x, y);
                    b.Init(b.color, pos);

                    blockDict[pos] = b;
                }
            }
        }
    }

    public void DropAndRefill()
    {
        // Ÿ�ϸ��� ���� ��������
        BoundsInt bounds = bgTilemap.cellBounds;

        // ��(col, x) ������ ó��
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            // �Ʒ����� ���� �˻�
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector2Int pos = new Vector2Int(x, y);

                // �� ĭ�� ���
                if (bgTilemap.HasTile(new Vector3Int(x, y, 0)) && !blockDict.ContainsKey(pos))
                {
                    // ���ʿ��� �� ã��
                    for (int ny = y + 1; ny < bounds.yMax; ny++)
                    {
                        Vector2Int abovePos = new Vector2Int(x, ny);

                        if (blockDict.TryGetValue(abovePos, out Block aboveBlock))
                        {
                            // ���� �Ʒ��� �̵�
                            blockDict.Remove(abovePos);
                            blockDict[pos] = aboveBlock;

                            aboveBlock.gridPos = pos;
                            Vector3 targetWorldPos = bgTilemap.GetCellCenterWorld(new Vector3Int(x, y, 0));

                            StartCoroutine(aboveBlock.MoveTo(targetWorldPos, 0.3f));

                            break; // �� �� �ϳ� �������� ���� ����
                        }
                    }
                }
            }

            // �� ������ Ȯ�� ��, �ֻ���� ��� ������ �� �� ����
            for (int y = bounds.yMax - 1; y >= bounds.yMin; y--)
            {
                Vector2Int pos = new Vector2Int(x, y);

                if (bgTilemap.HasTile(new Vector3Int(x, y, 0)) && !blockDict.ContainsKey(pos))
                {
                    // ���� �� ����
                    Block prefab = blockPrefabs[Random.Range(0, blockPrefabs.Length)];
                    Vector3 worldPos = bgTilemap.GetCellCenterWorld(new Vector3Int(x, y, 0));

                    Block newBlock = Instantiate(prefab, worldPos, Quaternion.identity);
                    newBlock.Init(newBlock.color, pos);

                    blockDict[pos] = newBlock;
                }
            }
        }
    }
}
