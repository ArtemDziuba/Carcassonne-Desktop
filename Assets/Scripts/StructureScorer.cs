using System.Collections.Generic;
using UnityEngine;

public static class StructureScorer
{
    // Індекси сегментів по сторонах: top, right, bottom, left
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

    /// <summary>
    /// Знаходить всі закриті структури (дороги, міста, монастирі),
    /// нараховує очки, повертає міплі і оновлює UI.
    /// </summary>
    public static void ScoreCompletedStructures(Board board,
                                            PlayerManager playerManager,
                                            PlayerUIManager uiManager)
    {
        var visited = new HashSet<(Vector2Int, int)>();

        foreach (var kvp in board.placedTiles)
        {
            var pos = kvp.Key;
            var tile = kvp.Value;
            var segments = tile.GetSegments();

            for (int id = 0; id < segments.Count; id++)
            {
                var seg = segments[id];
                if (!seg.HasMeeple || visited.Contains((pos, id)))
                    continue;

                var type = seg.Type;
                var structure = CollectStructure(board, pos, id, type);
                foreach (var s in structure) visited.Add(s);

                if (!IsStructureComplete(board, structure, type))
                    continue;

                int score = CalculateStructureScore(board, structure, type);
                var owners = FindStructureOwners(board, structure);

                foreach (var player in owners)
                {
                    // нараховуємо очки + оновлюємо UI
                    player.Score += score;
                    uiManager.UpdatePlayerScore(player.PlayerId, player.Score);

                    // повертаємо міпла (кількість + UI)
                    player.ReturnMeeple();
                    uiManager.UpdatePlayerMeeples(player.PlayerId, player.MeepleCount);
                }

                // **тут видаляємо міплів із поля**
                foreach (var (p, sid) in structure)
                {
                    var s2 = board.GetTileAt(p).GetSegments()[sid];
                    if (s2.MeepleObject != null)
                    {
                        Object.Destroy(s2.MeepleObject);
                        s2.MeepleObject = null;
                    }
                    s2.HasMeeple = false;
                    s2.MeepleOwner = null;
                }
            }
        }
    }


    // BFS/DFS для збору всіх сегментів однієї структури того ж типу
    private static HashSet<(Vector2Int, int)> CollectStructure(Board board,
                                                               Vector2Int startPos,
                                                               int startId,
                                                               TerrainType type)
    {
        var result = new HashSet<(Vector2Int, int)>();
        var q = new Queue<(Vector2Int, int)>();
        q.Enqueue((startPos, startId));

        while (q.Count > 0)
        {
            var (pos, id) = q.Dequeue();
            if (!result.Add((pos, id)))
                continue;

            var tile = board.GetTileAt(pos);
            var segs = tile.GetSegments();
            var seg = segs[id];

            // внутрішні зв’язки
            foreach (int conn in seg.ConnectedSegmentIds)
            {
                if (!result.Contains((pos, conn)) && segs[conn].Type == type)
                    q.Enqueue((pos, conn));
            }

            // зовнішній перехід по цьому одному сегменту
            int side = GetSideFromSegmentId(id);
            if (side == -1) continue;

            int[] mySide = sideIndices[side];
            int idx = System.Array.IndexOf(mySide, id);
            if (idx < 0) continue;

            Vector2Int np = pos + directions[side];
            var ntile = board.GetTileAt(np);
            if (ntile == null) continue;

            int opp = (side + 2) % 4;
            int[] theirSide = sideIndices[opp];
            int theirId = theirSide[2 - idx];

            if (ntile.GetSegments()[theirId].Type == type)
                q.Enqueue((np, theirId));
        }

        return result;
    }

    // Перевірка, чи структура замкнена згідно правил
    private static bool IsStructureComplete(Board board,
                                            HashSet<(Vector2Int, int)> structSegs,
                                            TerrainType type)
    {
        if (type == TerrainType.Monastery)
        {
            // монастир — перевіряємо 8 сусідніх тайлів
            foreach (var (pos, _) in structSegs)
            {
                int count = 0;
                foreach (var off in SurroundingOffsets())
                    if (board.GetTileAt(pos + off) != null)
                        count++;
                if (count == 8) return true;
            }
            return false;
        }

        // дороги й замки — жодного «відкритого» краю
        foreach (var (pos, id) in structSegs)
        {
            int side = GetSideFromSegmentId(id);
            if (side == -1) continue;

            int[] mySide = sideIndices[side];
            int idx = System.Array.IndexOf(mySide, id);
            if (idx < 0) continue;

            Vector2Int np = pos + directions[side];
            var ntile = board.GetTileAt(np);
            if (ntile == null) return false;

            int opp = (side + 2) % 4;
            int[] theirSide = sideIndices[opp];
            int theirId = theirSide[2 - idx];

            var neighSeg = ntile.GetSegments()[theirId];
            if (neighSeg.Type != type)
                return false;
        }

        return true;
    }

    // Нарахунок очок
    private static int CalculateStructureScore(Board board,
                                               HashSet<(Vector2Int, int)> structSegs,
                                               TerrainType type)
    {
        // Рахуємо унікальні тайли в структурі
        var tiles = new HashSet<Vector2Int>();
        foreach (var (pos, _) in structSegs)
            tiles.Add(pos);

        int score = 0;
        if (type == TerrainType.Road)
        {
            score = tiles.Count * 1;
        }
        else if (type == TerrainType.City)
        {
            // 2 p. за тайл + 2 p. за щит
            int shields = 0;
            foreach (var pos in tiles)
                if (board.GetTileAt(pos).Data.HasShield)
                    shields++;
            score = tiles.Count * 2 + shields * 2;
        }
        else if (type == TerrainType.Monastery)
        {
            score = 9;
        }

        return score;
    }

    // Хто був власником міпла(ів) у цій структурі?
    private static List<Player> FindStructureOwners(Board board,
                                                   HashSet<(Vector2Int, int)> structSegs)
    {
        var counts = new Dictionary<Player, int>();
        foreach (var (pos, id) in structSegs)
        {
            var seg = board.GetTileAt(pos).GetSegments()[id];
            if (seg.HasMeeple && seg.MeepleOwner != null)
            {
                if (!counts.ContainsKey(seg.MeepleOwner))
                    counts[seg.MeepleOwner] = 0;
                counts[seg.MeepleOwner]++;
            }
        }

        // Міняємо лише тих, хто поставив найбільше міплів (для простоти —
        // зазвичай це 1 міпл/структура)
        int max = 0;
        foreach (var v in counts.Values) if (v > max) max = v;

        var winners = new List<Player>();
        foreach (var kv in counts)
            if (kv.Value == max)
                winners.Add(kv.Key);

        return winners;
    }

    // Зсуви для монастирів
    private static List<Vector2Int> SurroundingOffsets() => new List<Vector2Int>
    {
        Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right,
        new Vector2Int(1,1), new Vector2Int(-1,1),
        new Vector2Int(1,-1), new Vector2Int(-1,-1)
    };

    private static int GetSideFromSegmentId(int id)
    {
        if (id <= 2) return 0;  // top
        if (id <= 5) return 1;  // right
        if (id <= 8) return 2;  // bottom
        if (id <= 11) return 3;  // left
        return -1;
    }
}
