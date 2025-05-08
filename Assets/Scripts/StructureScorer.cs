// StructureScorer.cs
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Обчислює очки за структури під час гри та в кінці гри.
/// </summary>
public static class StructureScorer
{
    private static readonly int[][] sideIndices = new[]
    {
        new[] {0,1,2},
        new[] {3,4,5},
        new[] {6,7,8},
        new[] {9,10,11}
    };
    private static readonly Vector2Int[] directions = new[]
    {
        Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left
    };
    private static readonly Vector2Int[] monasteryOffsets = new[]
    {
        new Vector2Int( 0, 1), new Vector2Int( 1, 1),
        new Vector2Int( 1, 0), new Vector2Int( 1,-1),
        new Vector2Int( 0,-1), new Vector2Int(-1,-1),
        new Vector2Int(-1, 0), new Vector2Int(-1, 1)
    };

    /// <summary>
    /// Нараховує очки одразу при замиканні структур під час гри.
    /// </summary>
    public static void ScoreCompletedStructures(
    Board board,
    PlayerManager pm,
    PlayerUIManager ui)
    {
        // 1) монастирі — незмінно
        foreach (var kv in board.placedTiles)
        {
            var pos = kv.Key;
            var tile = kv.Value;
            if (!tile.Data.HasMonastery || !tile.HasMonasteryMeeple)
                continue;

            int filled = monasteryOffsets.Count(off => board.GetTileAt(pos + off) != null);
            if (filled == 8)
            {
                var owner = tile.MonasteryMeepleOwner;
                owner.Score += 9;
                ui.UpdatePlayerScore(owner.PlayerId, owner.Score);

                // повертаємо один міпл монастиря
                owner.ReturnMeeple();
                ui.UpdatePlayerMeeples(owner.PlayerId, owner.MeepleCount);

                tile.ClearMonasteryMeeple();
            }
        }

        // 2) дороги та міста
        var visited = new HashSet<(Vector2Int, int)>();
        foreach (var kv in board.placedTiles)
        {
            var startPos = kv.Key;
            var tile = kv.Value;
            var segs = tile.GetSegments();

            for (int id = 0; id < segs.Count; id++)
            {
                var seg = segs[id];
                if (!seg.HasMeeple || visited.Contains((startPos, id)) || seg.Type == TerrainType.Monastery)
                    continue;

                var type = seg.Type;
                var structure = CollectStructure(board, startPos, id, type);
                foreach (var s in structure) visited.Add(s);

                if (!IsStructureComplete(board, structure, type))
                    continue;

                int points = CalculateCompletedScore(board, structure, type);

                // --- Ось головна зміна: рахуємо унікальні MeepleObject ---
                var meepleMap = new Dictionary<Player, HashSet<GameObject>>();
                foreach (var (pos, segId) in structure)
                {
                    var s2 = board.GetTileAt(pos).GetSegments()[segId];
                    if (s2.MeepleObject != null && s2.MeepleOwner != null)
                    {
                        if (!meepleMap.TryGetValue(s2.MeepleOwner, out var set))
                        {
                            set = new HashSet<GameObject>();
                            meepleMap[s2.MeepleOwner] = set;
                        }
                        set.Add(s2.MeepleObject);
                    }
                }

                if (meepleMap.Count == 0)
                    continue;

                int maxCount = meepleMap.Values.Max(set => set.Count);
                var winners = meepleMap
                    .Where(kv2 => kv2.Value.Count == maxCount)
                    .Select(kv2 => kv2.Key);

                // даємо очки всім, у кого Count == maxCount
                foreach (var p in winners)
                {
                    p.Score += points;
                    ui.UpdatePlayerScore(p.PlayerId, p.Score);
                }

                // повертаємо міплів — кожному рівно по числу об’єктів у його HashSet
                foreach (var kv2 in meepleMap)
                {
                    int toReturn = kv2.Value.Count;
                    for (int i = 0; i < toReturn; i++)
                        kv2.Key.ReturnMeeple();
                    ui.UpdatePlayerMeeples(kv2.Key.PlayerId, kv2.Key.MeepleCount);
                }

                // знищення об’єктів міплів та очищення сегментів
                var destroyed = new HashSet<GameObject>();
                foreach (var kv2 in meepleMap)
                {
                    foreach (var go in kv2.Value)
                    {
                        if (destroyed.Add(go))
                            Object.Destroy(go);
                    }
                }
                foreach (var (pos, segId) in structure)
                {
                    var s2 = board.GetTileAt(pos).GetSegments()[segId];
                    s2.HasMeeple = false;
                    s2.MeepleOwner = null;
                    s2.MeepleObject = null;
                }
            }
        }
    }


    /// <summary>
    /// Нараховує очки наприкінці гри за всі незакриті структури.
    /// </summary>
    public static void ScoreEndGameStructures(
        Board board,
        PlayerManager pm,
        PlayerUIManager ui)
    {
        ScoreIncompleteRoads(board, pm, ui);
        ScoreIncompleteCities(board, pm, ui);
        ScoreIncompleteMonasteries(board, pm, ui);
    }

    // ---- Helpers for End Game ----

    private static void ScoreIncompleteRoads(
        Board board,
        PlayerManager pm,
        PlayerUIManager ui)
    {
        var visited = new HashSet<(Vector2Int, int)>();
        foreach (var kv in board.placedTiles)
        {
            var pos = kv.Key;
            var segs = kv.Value.GetSegments();
            for (int id = 0; id < segs.Count; id++)
            {
                var seg = segs[id];
                if (seg.Type != TerrainType.Road || !seg.HasMeeple || visited.Contains((pos, id)))
                    continue;

                var structure = CollectStructure(board, pos, id, TerrainType.Road);
                foreach (var s in structure) visited.Add(s);

                int points = structure.Select(x => x.Item1).Distinct().Count();

                var winners = FindOwnersByMeepleCount(board, structure);
                foreach (var p in winners)
                {
                    p.Score += points;
                    ui.UpdatePlayerScore(p.PlayerId, p.Score);
                }

                ReturnAndDestroy(board, structure, pm, ui);
            }
        }
    }

    private static void ScoreIncompleteCities(
        Board board,
        PlayerManager pm,
        PlayerUIManager ui)
    {
        var visited = new HashSet<(Vector2Int, int)>();
        foreach (var kv in board.placedTiles)
        {
            var pos = kv.Key;
            var segs = kv.Value.GetSegments();
            for (int id = 0; id < segs.Count; id++)
            {
                var seg = segs[id];
                if (seg.Type != TerrainType.City || !seg.HasMeeple || visited.Contains((pos, id)))
                    continue;

                var structure = CollectStructure(board, pos, id, TerrainType.City);
                foreach (var s in structure) visited.Add(s);

                var tiles = structure.Select(x => x.Item1).Distinct();
                int tileCount = tiles.Count();
                int shieldCount = tiles.Count(p => board.GetTileAt(p).Data.HasShield);
                int points = tileCount + shieldCount;

                var winners = FindOwnersByMeepleCount(board, structure);
                foreach (var p in winners)
                {
                    p.Score += points;
                    ui.UpdatePlayerScore(p.PlayerId, p.Score);
                }

                ReturnAndDestroy(board, structure, pm, ui);
            }
        }
    }

    private static void ScoreIncompleteMonasteries(
        Board board,
        PlayerManager pm,
        PlayerUIManager ui)
    {
        foreach (var kv in board.placedTiles)
        {
            var pos = kv.Key;
            var tile = kv.Value;
            if (!tile.Data.HasMonastery || !tile.HasMonasteryMeeple)
                continue;

            int filled = monasteryOffsets.Count(off => board.GetTileAt(pos + off) != null);
            int points = filled + 1;

            var owner = tile.MonasteryMeepleOwner;
            owner.Score += points;
            ui.UpdatePlayerScore(owner.PlayerId, owner.Score);

            owner.ReturnMeeple();
            ui.UpdatePlayerMeeples(owner.PlayerId, owner.MeepleCount);

            tile.ClearMonasteryMeeple();
        }
    }

    // ---- Shared Utilities ----

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
            foreach (var conn in segs[id].ConnectedSegmentIds)
                if (segs[conn].Type == type)
                    queue.Enqueue((pos, conn));

            int side = GetSideFromSegmentId(id);
            if (side < 0) continue;
            var nPos = pos + directions[side];
            var neighbor = board.GetTileAt(nPos);
            if (neighbor == null) continue;
            int idx = System.Array.IndexOf(sideIndices[side], id);
            int opp = (side + 2) % 4;
            int their = sideIndices[opp][2 - idx];
            if (neighbor.GetSegments()[their].Type == type)
                queue.Enqueue((nPos, their));
        }
        return result;
    }

    private static bool IsStructureComplete(
        Board board,
        HashSet<(Vector2Int, int)> segs,
        TerrainType type)
    {
        foreach (var (pos, id) in segs)
        {
            int side = GetSideFromSegmentId(id);
            if (side < 0) continue;
            var nPos = pos + directions[side];
            var neighbor = board.GetTileAt(nPos);
            if (neighbor == null) return false;
            int idx = System.Array.IndexOf(sideIndices[side], id);
            int opp = (side + 2) % 4;
            int their = sideIndices[opp][2 - idx];
            if (neighbor.GetSegments()[their].Type != type)
                return false;
        }
        return true;
    }

    private static int CalculateCompletedScore(
        Board board,
        HashSet<(Vector2Int, int)> segs,
        TerrainType type)
    {
        var tiles = segs.Select(x => x.Item1).Distinct().ToList();
        switch (type)
        {
            case TerrainType.Road:
                return tiles.Count;
            case TerrainType.City:
                {
                    int shields = tiles.Count(p => board.GetTileAt(p).Data.HasShield);
                    return tiles.Count * 2 + shields * 2;
                }
            default:
                return 0;
        }
    }

    private static IEnumerable<Player> FindOwnersByMeepleCount(
        Board board,
        HashSet<(Vector2Int, int)> segs)
    {
        var counts = new Dictionary<Player, int>();
        foreach (var (pos, id) in segs)
        {
            var s = board.GetTileAt(pos).GetSegments()[id];
            if (s.HasMeeple && s.MeepleOwner != null)
                counts[s.MeepleOwner] = counts.GetValueOrDefault(s.MeepleOwner) + 1;
        }
        int max = counts.Values.DefaultIfEmpty(0).Max();
        return counts.Where(kv => kv.Value == max).Select(kv => kv.Key);
    }

    /// <summary>
    /// Повернути кожному гравцю рівно ту кількість міплів, які він поставив,
    /// потім знищити їхні об’єкти на полі.
    /// </summary>
    private static void ReturnAndDestroy(
        Board board,
        HashSet<(Vector2Int, int)> segs,
        PlayerManager pm,
        PlayerUIManager ui)
    {
        // 1. Збираємо унікальні об’єкти міплів по гравцях
        var map = new Dictionary<Player, HashSet<GameObject>>();
        foreach (var (pos, id) in segs)
        {
            var s = board.GetTileAt(pos).GetSegments()[id];
            if (s.MeepleObject != null && s.MeepleOwner != null)
            {
                if (!map.TryGetValue(s.MeepleOwner, out var set))
                {
                    set = new HashSet<GameObject>();
                    map[s.MeepleOwner] = set;
                }
                set.Add(s.MeepleObject);
            }
        }

        // 2. Повернення міплів
        foreach (var kv in map)
        {
            int cnt = kv.Value.Count;
            for (int i = 0; i < cnt; i++)
                kv.Key.ReturnMeeple();
            ui.UpdatePlayerMeeples(kv.Key.PlayerId, kv.Key.MeepleCount);
        }

        // 3. Знищення об’єктів та очищення сегментів
        var destroyed = new HashSet<GameObject>();
        foreach (var (pos, id) in segs)
        {
            var s = board.GetTileAt(pos).GetSegments()[id];
            if (s.MeepleObject != null && destroyed.Add(s.MeepleObject))
                Object.Destroy(s.MeepleObject);
            s.HasMeeple = false;
            s.MeepleOwner = null;
            s.MeepleObject = null;
        }
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
