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

    [Header("Goal UI")]
    [SerializeField] private RectTransform goalUI;

    public List<JackBox> jackBoxes = new List<JackBox>();
    private void Start()
    {
        SpawnJackBoxesFromTile();

        jackBoxes = new List<JackBox>(FindObjectsByType<JackBox>(FindObjectsSortMode.None));
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

                    GameObject go = Instantiate(jackBoxPrefab, worldPos, Quaternion.identity);
                    JackBox jack = go.GetComponent<JackBox>();
                    jack.Initialize(goalUI);
                    jackBoxes.Add(jack);
                    jackBoxTilemap.SetTile(cellPos, null);

                }
            }
        }
    }
}
