using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class HexBoardSpawner : MonoBehaviour
{
    [Header("Tilemap")]
    public Tilemap bgTilemap;           // 하트 모양으로 칠해둔 배경 타일맵
    

    [Header("Blocks")]
    public Block[] blockPrefabs;        // 블럭 프리팹(색상별)

    public MatchManager matchManager;
    

    public Dictionary<Vector2Int, Block> blockDict = new Dictionary<Vector2Int, Block>();

    void Start()
    {
        SpawnBlocks();
    }

    void SpawnBlocks()
    {
        // Tilemap의 BoundsInt 구하기
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
        // 타일맵의 범위 가져오기
        BoundsInt bounds = bgTilemap.cellBounds;

        // 열(col, x) 단위로 처리
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            // 아래에서 위로 검사
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector2Int pos = new Vector2Int(x, y);

                // 빈 칸인 경우
                if (bgTilemap.HasTile(new Vector3Int(x, y, 0)) && !blockDict.ContainsKey(pos))
                {
                    // 위쪽에서 블럭 찾기
                    for (int ny = y + 1; ny < bounds.yMax; ny++)
                    {
                        Vector2Int abovePos = new Vector2Int(x, ny);

                        if (blockDict.TryGetValue(abovePos, out Block aboveBlock))
                        {
                            // 블럭을 아래로 이동
                            blockDict.Remove(abovePos);
                            blockDict[pos] = aboveBlock;

                            aboveBlock.gridPos = pos;
                            Vector3 targetWorldPos = bgTilemap.GetCellCenterWorld(new Vector3Int(x, y, 0));

                            StartCoroutine(aboveBlock.MoveTo(targetWorldPos, 0.3f));

                            break; // 위 블럭 하나 내렸으면 루프 종료
                        }
                    }
                }
            }

            // 맨 위까지 확인 후, 최상단이 비어 있으면 새 블럭 생성
            for (int y = bounds.yMax - 1; y >= bounds.yMin; y--)
            {
                Vector2Int pos = new Vector2Int(x, y);

                if (bgTilemap.HasTile(new Vector3Int(x, y, 0)) && !blockDict.ContainsKey(pos))
                {
                    // 랜덤 블럭 생성
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
