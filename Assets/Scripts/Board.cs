using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public Dictionary<Vector2Int, Tile> placedTiles = new Dictionary<Vector2Int, Tile>();

    public bool CanPlaceTileAt(Vector2Int position, TileData tileData, int rotation)
    {
        // Перевірити, чи є сусіди
        // І чи сторони сумісні
        return true; // тимчасово
    }

    public void PlaceTile(Vector2Int position, Tile tile)
    {
        placedTiles[position] = tile;
        Vector3 newPosition = tile.transform.position;
        newPosition.x = position.x;
        newPosition.y = position.y;
        tile.transform.position = newPosition;
    }

    public Tile GetTileAt(Vector2Int position)
    {
        return placedTiles.ContainsKey(position) ? placedTiles[position] : null;
    }
}
