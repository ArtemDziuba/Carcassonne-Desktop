using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class StructureScorer
{
    private static readonly int[][] sideIndices = new int[][]
    {
        new[] {0,1,2},
        new[] {3,4,5},
        new[] {6,7,8},
        new[] {9,10,11}
    };

    private static readonly Vector2Int[] directions = new Vector2Int[]
    {
        Vector2Int.up,
        Vector2Int.right,
        Vector2Int.down,
        Vector2Int.left
    };

    private static readonly Vector2Int[] monasteryOffsets = new Vector2Int[]
    {
        new Vector2Int( 0, 1), new Vector2Int( 1, 1),
        new Vector2Int( 1, 0), new Vector2Int( 1,-1),
        new Vector2Int( 0,-1), new Vector2Int(-1,-1),
        new Vector2Int(-1, 0), new Vector2Int(-1, 1)
    };

    public static void ScoreCompletedStructures(
        Board board,
        PlayerManager playerManager,
        PlayerUIManager uiManager)
    {
        // 1) Монастирі
        foreach (var kvp in board.placedTiles)
        {
            Vector2Int pos = kvp.Key;
            Tile tile = kvp.Value;

            if (!tile.Data.HasMonastery || !tile.HasMonasteryMeeple)
                continue;

            int count = monasteryOffsets
                .Count(off => board.GetTileAt(pos + off) != null);

            if (count == 8)
            {
                Player owner = tile.MonasteryMeepleOwner;
                owner.Score += 9;
                uiManager.UpdatePlayerScore(owner.PlayerId, owner.Score);
                owner.ReturnMeeple();
                uiManager.UpdatePlayerMeeples(owner.PlayerId, owner.MeepleCount);
                tile.ClearMonasteryMeeple();
            }
        }

        // 2) Дороги і міста
        var visited = new HashSet<(Vector2Int, int)>();

        foreach (var kvp in board.placedTiles)
        {
            Vector2Int pos = kvp.Key;
            Tile tile = kvp.Value;
            List<Segment> segs = tile.GetSegments();

            for (int id = 0; id < segs.Count; id++)
            {
                Segment seg = segs[id];
                if (!seg.HasMeeple || visited.Contains((pos, id)))
                    continue;

                TerrainType type = seg.Type;
                var structure = CollectStructure(board, pos, id, type);
                foreach (var s in structure) visited.Add(s);

                if (!IsStructureComplete(board, structure, type))
                    continue;

                int score = CalculateStructureScore(board, structure, type);
                var owners = FindStructureOwners(board, structure);
                foreach (var p in owners)
                {
                    p.Score += score;
                    uiManager.UpdatePlayerScore(p.PlayerId, p.Score);
                    p.ReturnMeeple();
                    uiManager.UpdatePlayerMeeples(p.PlayerId, p.MeepleCount);
                }

                // Видалення міплів
                foreach (var (p, sid) in structure)
                {
                    var s2 = board.GetTileAt(p).GetSegments()[sid];
                    if (s2.MeepleObject != null)
                        Object.Destroy(s2.MeepleObject);
                    s2.HasMeeple = false;
                    s2.MeepleOwner = null;
                    s2.MeepleObject = null;
                }
            }
        }
    }

    private static HashSet<(Vector2Int, int)> CollectStructure(
        Board board,
        Vector2Int startPos,
        int startId,
        TerrainType type)
    {
        var result = new HashSet<(Vector2Int, int)>();
        var queue = new Queue<(Vector2Int, int)>();
        queue.Enqueue((startPos, startId));

        while (queue.Count > 0)
        {
            var (pos, id) = queue.Dequeue();
            if (!result.Add((pos, id))) continue;

            var segs = board.GetTileAt(pos).GetSegments();
            // внутрішні зв’язки
            foreach (int conn in segs[id].ConnectedSegmentIds)
                if (segs[conn].Type == type)
                    queue.Enqueue((pos, conn));

            // зовнішній перехід
            int side = GetSideFromSegmentId(id);
            if (side < 0) continue;

            int idx = System.Array.IndexOf(sideIndices[side], id);
            Vector2Int neighPos = pos + directions[side];
            Tile neighTile = board.GetTileAt(neighPos);
            if (neighTile == null) continue;

            int opp = (side + 2) % 4;
            int theirId = sideIndices[opp][2 - idx];
            if (neighTile.GetSegments()[theirId].Type == type)
                queue.Enqueue((neighPos, theirId));
        }

        return result;
    }

    private static bool IsStructureComplete(
        Board board,
        HashSet<(Vector2Int, int)> structure,
        TerrainType type)
    {
        // для доріг і міст — жодного відкритого краю
        foreach (var (pos, id) in structure)
        {
            int side = GetSideFromSegmentId(id);
            if (side < 0) continue;

            int idx = System.Array.IndexOf(sideIndices[side], id);
            Vector2Int neighPos = pos + directions[side];
            Tile neighTile = board.GetTileAt(neighPos);
            if (neighTile == null) return false;

            int opp = (side + 2) % 4;
            int their = sideIndices[opp][2 - idx];
            if (neighTile.GetSegments()[their].Type != type)
                return false;
        }

        return true;
    }

    private static int CalculateStructureScore(
        Board board,
        HashSet<(Vector2Int, int)> structure,
        TerrainType type)
    {
        var tiles = new HashSet<Vector2Int>();
        foreach (var (p, _) in structure)
            tiles.Add(p);

        switch (type)
        {
            case TerrainType.Road:
                return tiles.Count;
            case TerrainType.City:
                int shields = tiles.Count(p => board.GetTileAt(p).Data.HasShield);
                return tiles.Count * 2 + shields * 2;
            default:
                return 0;
        }
    }

    private static List<Player> FindStructureOwners(
        Board board,
        HashSet<(Vector2Int, int)> structure)
    {
        var counts = new Dictionary<Player, int>();
        foreach (var (p, id) in structure)
        {
            var seg = board.GetTileAt(p).GetSegments()[id];
            if (seg.HasMeeple && seg.MeepleOwner != null)
                counts[seg.MeepleOwner] = counts.GetValueOrDefault(seg.MeepleOwner) + 1;
        }

        int max = counts.Values.DefaultIfEmpty(0).Max();
        return counts
            .Where(kv => kv.Value == max)
            .Select(kv => kv.Key)
            .ToList();
    }

    private static int GetSideFromSegmentId(int id)
    {
        if (id <= 2) return 0;
        if (id <= 5) return 1;
        if (id <= 8) return 2;
        if (id <= 11) return 3;
        return -1;
    }
}