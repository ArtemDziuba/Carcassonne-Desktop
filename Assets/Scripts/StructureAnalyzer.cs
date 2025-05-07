using System.Collections.Generic;
using UnityEngine;

public static class StructureAnalyzer
{
    private static readonly int[][] sideIndices = new int[][]
    {
        new int[] { 0, 1, 2 },    // top
        new int[] { 3, 4, 5 },    // right
        new int[] { 6, 7, 8 },    // bottom
        new int[] { 9, 10, 11 }   // left
    };

    private static readonly Vector2Int[] directions = new Vector2Int[]
    {
        Vector2Int.up,
        Vector2Int.right,
        Vector2Int.down,
        Vector2Int.left
    };


    public static bool IsStructureOccupied(Board board, Vector2Int tilePos, List<int> segmentIds)
    {
        if (segmentIds == null || segmentIds.Count == 0)
        {
            Debug.LogWarning("[StructureAnalyzer] segmentIds порожній — структура автоматично вважається незайнятою.");
            return false;
        }

        var visited = new HashSet<(Vector2Int pos, int seg)>();
        var queue = new Queue<(Vector2Int pos, int seg)>();

        Tile startTile = board.GetTileAt(tilePos);
        if (startTile == null) return false;

        var startSegments = startTile.GetSegments();
        int firstId = segmentIds[0];
        if (firstId < 0 || firstId >= startSegments.Count)
        {
            Debug.LogWarning("[StructureAnalyzer] Початковий segmentId поза межами.");
            return false;
        }

        TerrainType targetType = startSegments[firstId].Type;

        // enqueue всіх стартових сегментів
        foreach (int segId in segmentIds)
            queue.Enqueue((tilePos, segId));

        while (queue.Count > 0)
        {
            var (pos, id) = queue.Dequeue();
            if (!visited.Add((pos, id)))
                continue;

            Tile tile = board.GetTileAt(pos);
            if (tile == null)
                continue;

            var segments = tile.GetSegments();
            if (id < 0 || id >= segments.Count)
                continue;

            var segment = segments[id];
            if (segment.Type != targetType)
                continue;

            Debug.Log($"[BFS] Visiting tile {pos}, segId={id}, type={segment.Type}");

            if (segment.HasMeeple)
                return true;

            // внутрішні переходи
            foreach (int connId in segment.ConnectedSegmentIds)
            {
                if (!visited.Contains((pos, connId)) &&
                    segments[connId].Type == targetType)
                {
                    queue.Enqueue((pos, connId));
                }
            }

            // зовнішній перехід тільки по цьому id, якщо він лежить на грані
            int side = GetSideFromSegmentId(id);
            if (side == -1)
                continue;

            // знайдемо позицію id у межах цієї грані
            int[] mySide = sideIndices[side];
            int indexOnSide = System.Array.IndexOf(mySide, id);
            if (indexOnSide < 0)
                continue;

            Vector2Int neighborPos = pos + directions[side];
            Tile neighborTile = board.GetTileAt(neighborPos);
            if (neighborTile == null)
                continue;

            var neighborSegments = neighborTile.GetSegments();
            int oppositeSide = (side + 2) % 4;
            int[] theirSide = sideIndices[oppositeSide];
            int theirSegId = theirSide[2 - indexOnSide];

            Debug.Log($"[BFS]  External: side={side}, mySeg={id}({segment.Type}) ? " +
                      $"neighbor {neighborPos}, seg={theirSegId}({neighborSegments[theirSegId].Type})");

            if (neighborSegments[theirSegId].Type == targetType &&
                !visited.Contains((neighborPos, theirSegId)))
            {
                queue.Enqueue((neighborPos, theirSegId));
            }
        }

        return false;
    }



    private static int GetSideFromSegmentId(int id)
    {
        if (id >= 0 && id <= 2) return 0; // top
        if (id >= 3 && id <= 5) return 1; // right
        if (id >= 6 && id <= 8) return 2; // bottom
        if (id >= 9 && id <= 11) return 3; // left
        return -1;
    }

    private static int[] GetRotatedSideIndices(int side, int rotation)
    {
        int steps = (rotation / 90) % 4;
        int rotatedSide = (side + 4 - steps) % 4; // обернено, бо координати обертаються проти год. стрілки
        return sideIndices[rotatedSide];
    }
}
