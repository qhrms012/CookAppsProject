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

                    // Block ��ũ��Ʈ�� GridPos �����صθ� ���� ��Ī/����� ����
                    b.Init(b.color, new Vector2Int(x, y));
                }
            }
        }
    }
}
