using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;

public class Shadows : MonoBehaviour
{
    public Tilemap tilemap;
    public GameObject shadowPrefab;

    private Dictionary<Vector3Int, GameObject> shadowTiles = new Dictionary<Vector3Int, GameObject>();

    void Start()
    {
        GetComponent<CompositeShadowCaster2D>().enabled = true;
        GenerateShadows();
    }

    void GenerateShadows()
    {
        // Loop through all tiles in the tilemap
        foreach (Vector3Int pos in tilemap.cellBounds.allPositionsWithin)
        {
            TileBase tile = tilemap.GetTile(pos);

            if (tile != null)
            {
                // Calculate the position of the shadow tile
                Vector3 shadowPos = tilemap.GetCellCenterWorld(pos);
                shadowPos.z = 0f;

                // Instantiate the shadow prefab at the calculated position
                GameObject shadow = Instantiate(shadowPrefab, shadowPos, Quaternion.identity, transform);

                // Add the shadow tile to the dictionary
                shadowTiles.Add(pos, shadow);
            }
        }
    }
}
