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

                    // Block 스크립트에 GridPos 저장해두면 추후 매칭/드랍에 유용
                    b.Init(b.color, new Vector2Int(x, y));
                }
            }
        }
    }
}
