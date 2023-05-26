using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "RandomRuleTile", menuName = "2D/Tiles/Random Rule Tile")]
public class RandomRuleTile : RuleTile
{
    public TileBase[] randomTiles;

    /*public override TileBase GetTile(Vector3Int position, ITilemap tilemap)
    {
        if (randomTiles != null && randomTiles.Length > 0)
        {
            int randomIndex = Random.Range(0, randomTiles.Length);
            return randomTiles[randomIndex];
        }

        return base.GetTile(position, tilemap);
    }
    */
}
