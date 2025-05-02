using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public Dictionary<Vector2Int, Tile> placedTiles = new();

    public bool CanPlaceTileAt(Vector2Int position, TileData tileData, int rotation)
    {
        if (placedTiles.ContainsKey(position))
            return false;

        List<Segment> testSegments = CloneAndRotateSegments(tileData.Segments, rotation);

        foreach (var dir in new (Vector2Int offset, int sideThis, int sideOther)[]
        {
            (Vector2Int.up,    0, 6),
            (Vector2Int.right, 3, 9),
            (Vector2Int.down,  6, 0),
            (Vector2Int.left,  9, 3),
        })
        {
            Vector2Int neighborPos = position + dir.offset;
            if (placedTiles.TryGetValue(neighborPos, out Tile neighborTile))
            {
                var segThis = testSegments[dir.sideThis];
                var segOther = neighborTile.GetSegments()[dir.sideOther];
                if (segThis.Type != segOther.Type)
                {
                    return false;
                }
            }
        }

        return true;
    }

    private List<Segment> CloneAndRotateSegments(List<Segment> source, int degrees)
    {
        int shift = 3 * (degrees / 90);
        var newSegments = new List<Segment>(new Segment[12]);

        for (int i = 0; i < 12; i++)
        {
            int newIndex = (i + shift) % 12;
            Segment oldSeg = source[i];

            Segment rotated = new Segment
            {
                Id = newIndex,
                Type = oldSeg.Type,
                HasMeeple = oldSeg.HasMeeple,
                MeepleOwner = oldSeg.MeepleOwner,
                ConnectedSegmentIds = new List<int>()
            };

            newSegments[newIndex] = rotated;
        }

        for (int i = 0; i < 12; i++)
        {
            int oldIndex = (i - shift + 12) % 12;
            Segment oldSeg = source[oldIndex];
            Segment newSeg = newSegments[i];

            foreach (var id in oldSeg.ConnectedSegmentIds)
            {
                newSeg.ConnectedSegmentIds.Add((id + shift) % 12);
            }
        }

        return newSegments;
    }

    public void PlaceTile(Vector2Int position, Tile tile)
    {
        placedTiles[position] = tile;
        tile.transform.position = new Vector3(position.x, position.y, 0);
    }

    public Tile GetTileAt(Vector2Int position)
    {
        return placedTiles.TryGetValue(position, out var tile) ? tile : null;
    }

    public HashSet<Vector2Int> GetValidPositions()
    {
        HashSet<Vector2Int> valid = new();

        foreach (var pos in placedTiles.Keys)
        {
            foreach (var dir in new Vector2Int[] {
                Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right })
            {
                Vector2Int adjacent = pos + dir;
                if (!placedTiles.ContainsKey(adjacent))
                    valid.Add(adjacent);
            }
        }

        return valid;
    }
}
