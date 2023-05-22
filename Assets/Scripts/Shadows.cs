using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;

public class Shadows : MonoBehaviour
{
    public Tilemap tilemap;
    public GameObject shadowPrefab;

    private CompositeShadowCaster2D compositeShadowCaster;
    private Dictionary<Vector3Int, GameObject> shadowTiles = new Dictionary<Vector3Int, GameObject>();

    void Start()
    {
        compositeShadowCaster = GetComponent<CompositeShadowCaster2D>();
        compositeShadowCaster.enabled = true;
        GenerateShadows();
    }

    void GenerateShadows()
    {
        // Loop through all tiles in the tilemap
        foreach (Vector3Int pos in tilemap.cellBounds.allPositionsWithin)
        {
            TileBase tile = tilemap.GetTile(pos);
            bool isNearPlayableArea = Physics2D.CircleCast((Vector3)pos, 0.6f, Vector2.right, 0.6f, LayerMask.GetMask("Ground"));

            if (tile != null && isNearPlayableArea)
            {
                // Check if the shadow tile is already created
                if (!shadowTiles.ContainsKey(pos))
                {
                    // Calculate the position of the shadow tile
                    Vector3 shadowPos = tilemap.GetCellCenterWorld(pos);
                    shadowPos.z = 0f;

                    // Instantiate the shadow prefab at the calculated position
                    GameObject shadow = Instantiate(shadowPrefab, shadowPos, Quaternion.identity, transform);
                    shadowTiles.Add(pos, shadow);

                    // Set the parent of the shadow object to the composite shadow caster
                    shadow.transform.SetParent(compositeShadowCaster.transform, true);
                }
            }
        }
    }

    void LateUpdate()
    {
        // Perform frustum culling on shadow objects that are no longer visible
        foreach (var shadowTile in shadowTiles)
        {
            Vector3 shadowPos = tilemap.GetCellCenterWorld(shadowTile.Key);
            shadowPos.z = 0f;

            bool isVisible = IsVisible(shadowPos);
            shadowTile.Value.SetActive(isVisible);
        }
    }

    bool IsVisible(Vector3 position)
    {
        Camera mainCamera = Camera.main;

        Vector3 viewportPos = mainCamera.WorldToViewportPoint(position);
        return viewportPos.x >= 0 && viewportPos.x <= 1 && viewportPos.y >= 0 && viewportPos.y <= 1 && viewportPos.z > 0;
    }
}
