using System.Collections.Generic;
using UnityEngine;

public static class StructureAnalyzer
{
    private static readonly int[][] sideIndices = new int[][]
    {
new int[] { 0, 1, 2 }, // top
new int[] { 3, 4, 5 }, // right
new int[] { 6, 7, 8 }, // bottom
new int[] { 9, 10, 11 } // left
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
            Debug.LogWarning("[StructureAnalyzer] segmentIds порожн≥й Ч структура автоматично вважаЇтьс€ незайн€тою.");
            return false;
        }

        HashSet<(Vector2Int, int)> visited = new();
        Queue<(Vector2Int, int)> queue = new();

        Tile startTile = board.GetTileAt(tilePos);
        if (startTile == null) return false;

        List<Segment> startSegments = startTile.GetSegments();
        if (segmentIds[0] < 0 || segmentIds[0] >= startSegments.Count)
        {
            Debug.LogWarning("[StructureAnalyzer] ѕочатковий segmentId поза межами.");
            return false;
        }

        Segment firstSegment = startSegments[segmentIds[0]];
        TerrainType targetType = firstSegment.Type;

        foreach (int segId in segmentIds)
            queue.Enqueue((tilePos, segId));

        while (queue.Count > 0)
        {
            var (pos, id) = queue.Dequeue();
            if (!visited.Add((pos, id))) continue;

            Tile tile = board.GetTileAt(pos);
            if (tile == null) continue;

            List<Segment> segments = tile.GetSegments();
            if (id < 0 || id >= segments.Count) continue;

            Segment segment = segments[id];
            if (segment.Type != targetType) continue;

            if (segment.HasMeeple)
                return true;

            // ¬нутр≥шн≥ переходи Ч з'Їднан≥ сегменти того ж типу
            foreach (int connId in segment.ConnectedSegmentIds)
            {
                if (connId >= 0 && connId < segments.Count)
                {
                    Segment connected = segments[connId];
                    if (connected.Type == targetType && !visited.Contains((pos, connId)))
                        queue.Enqueue((pos, connId));
                }
            }

            // «овн≥шн≥й перех≥д Ч сус≥дн≥й тайл по сторон≥
            int side = GetSideFromSegmentId(id);
            if (side == -1) continue;

            Vector2Int neighborPos = pos + directions[side];
            Tile neighborTile = board.GetTileAt(neighborPos);
            if (neighborTile == null) continue;

            List<Segment> neighborSegments = neighborTile.GetSegments();
            int[] neighborSide = sideIndices[(side + 2) % 4]; // протилежна сторона

            // ƒл€ пол≥в Ч перев≥рити одразу, чи Ї на поточн≥й сторон≥ хоч одна дорога
            if (targetType == TerrainType.Field)
            {
                int[] currentSide = sideIndices[side];
                for (int j = 0; j < 3; j++)
                {
                    int sid = currentSide[j];
                    if (segments[sid].Type == TerrainType.Road)
                    {
                        // якщо хоч один сегмент Ч дорога, то вс€ сторона роз≥рвана
                        continue; // не переходимо до сус≥да на ц≥й сторон≥
                    }
                }
            }

            // якщо не було continue, переходимо до сегмент≥в сус≥днього тайла
            for (int i = 0; i < 3; i++)
            {
                int neighborId = neighborSide[i];
                if (neighborId < 0 || neighborId >= neighborSegments.Count) continue;

                Segment neighborSegment = neighborSegments[neighborId];

                if (neighborSegment.Type == targetType && !visited.Contains((neighborPos, neighborId)))
                    queue.Enqueue((neighborPos, neighborId));
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
}