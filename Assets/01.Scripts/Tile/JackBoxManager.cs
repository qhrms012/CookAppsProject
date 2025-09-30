using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class JackBoxManager : MonoBehaviour
{
    [Header("JackboxTile")]
    public Tilemap jackBoxTilemap;

    [Header("JackBox")]
    public GameObject jackBoxPrefab;
    private void Start()
    {
        SpawnJackBoxesFromTile();
    }
    void SpawnJackBoxesFromTile()
    {
        BoundsInt bounds = jackBoxTilemap.cellBounds;

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int cellPos = new Vector3Int(x, y, 0);

                if (jackBoxTilemap.HasTile(cellPos))
                {

                    Vector3 worldPos = jackBoxTilemap.GetCellCenterWorld(cellPos);

                    Instantiate(jackBoxPrefab, worldPos, Quaternion.identity);
                    jackBoxTilemap.SetTile(cellPos, null);

                }
            }
        }
    }
}
