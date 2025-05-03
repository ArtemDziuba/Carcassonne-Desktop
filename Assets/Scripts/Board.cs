using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public Dictionary<Vector2Int, Tile> placedTiles = new();
    public Dictionary<Vector2Int, ShadowTileData> shadowTiles = new();

    public bool CanPlaceTileAt(Vector2Int position, TileData tileData, int rotation)
    {
        if (placedTiles.ContainsKey(position)) return false;
        if (!shadowTiles.TryGetValue(position, out var shadow)) return false;

        return shadow.Matches(tileData, rotation);
    }

    public void PlaceTile(Vector2Int position, Tile tile)
    {
        if (tile == null || tile.Data == null)
        {
            Debug.LogError($"Неможливо розмістити тайл у позиції {position}: Tile або Tile.Data дорівнює null.");
            return;
        }

        if (placedTiles.ContainsKey(position))
        {
            Debug.LogError($"Спроба розмістити тайл у вже зайнятій позиції {position}.");
            return;
        }

        placedTiles[position] = tile;
        tile.transform.position = new Vector3(position.x, position.y, 0);

        List<Segment> segments = tile.GetSegments();
        if (segments == null || segments.Count != 12)
        {
            Debug.LogError($"Помилка сегментів у тайлі на позиції {position}: GetSegments() повертає некоректні дані.");
            return;
        }

        shadowTiles.Remove(position);

        GenerateShadowsAround(position);
    }

    public void GenerateShadowsAround(Vector2Int position)
    {
        if (!placedTiles.TryGetValue(position, out Tile tile)) return;

        List<Segment> segments = tile.GetSegments();

        foreach (var dir in new Vector2Int[] {
        Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left })
        {
            Vector2Int neighborPos = position - dir;

            int thisSideStart = GetSideIndex(dir);
            int neighborSideStart = GetSideIndex(-dir);

            if (!shadowTiles.TryGetValue(neighborPos, out var shadow))
            {
                shadow = new ShadowTileData(neighborPos);
                shadowTiles[neighborPos] = shadow;
            }

            for (int i = 0; i < 3; i++)
            {
                Segment sourceSegment = segments[neighborSideStart + i];
                if (sourceSegment != null)
                {
                    shadow.SetSegment(thisSideStart + i, sourceSegment.Type);
                }
            }
        }
    }



    public ShadowTileData GetShadowAt(Vector2Int position)
    {
        return shadowTiles.TryGetValue(position, out var shadow) ? shadow : null;
    }

    public IEnumerable<Vector2Int> GetAllShadowPositions()
    {
        return shadowTiles.Keys;
    }

    public Tile GetTileAt(Vector2Int position)
    {
        return placedTiles.TryGetValue(position, out var tile) ? tile : null;
    }

    private int GetSideIndex(Vector2Int dir)
    {
        if (dir == Vector2Int.up) return 0;
        if (dir == Vector2Int.right) return 3;
        if (dir == Vector2Int.down) return 6;
        if (dir == Vector2Int.left) return 9;
        return -1;
    }
}
