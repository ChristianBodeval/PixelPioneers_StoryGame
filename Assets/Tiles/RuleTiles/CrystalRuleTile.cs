using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class CystalRuleTile : RuleTile<CystalRuleTile.Neighbor>
{
    public bool customField;

    public class Neighbor : RuleTile.TilingRule.Neighbor
    {
        public const int Null = 3;
        public const int NotNull = 4;
    }

    public Tile[] randomTiles; // Use Tile[] instead of TileBase[] if you're using Unity's built-in Tile class

    public override bool RuleMatch(int neighbor, TileBase tile)
    {
        if (randomTiles != null && randomTiles.Length > 0)
        {
            switch (neighbor)
            {
                case Neighbor.Null: return tile == null;
                case Neighbor.NotNull: return tile != null;
            }
        }
        return base.RuleMatch(neighbor, tile);
    }

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        if (randomTiles != null && randomTiles.Length > 0)
        {
            int randomIndex = Random.Range(0, randomTiles.Length);
            tileData.sprite = randomTiles[randomIndex].sprite;
        }
        else
        {
            base.GetTileData(position, tilemap, ref tileData);
        }
    }
}
