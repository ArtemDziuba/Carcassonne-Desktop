using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SaveSystem
{
    private static readonly string saveFolder = Path.Combine(Application.persistentDataPath, "Saves");

    public static void SaveGame(
    string saveId,
    Board board,
    PlayerManager playerManager,
    TileDeckManager deckManager,
    TurnManager turnManager)
    {
        if (!Directory.Exists(saveFolder))
            Directory.CreateDirectory(saveFolder);

        // 1) Ініціалізуємо snapshot з фазою ходу
        GameSnapshot snapshot = new GameSnapshot
        {
            saveName = saveId,
            date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            placedTiles = new List<PlacedTileData>(),
            players = new List<PlayerSaveData>(),
            currentPlayerIndex = playerManager.CurrentPlayerIndex,
            remainingTiles = new List<string>(),
            isFieldEnabled = GameConfig.Instance?.IsFieldEnabled ?? true,
            tilePlaced = turnManager.tilePlaced,
            meeplePlaced = turnManager.meeplePlaced
        };

        // 2) Зберігаємо тайли на полі
        foreach (var pair in board.placedTiles)
        {
            Tile tile = pair.Value;
            Vector2Int pos = pair.Key;

            var placed = new PlacedTileData
            {
                tileName = tile.Data.name,
                position = pos,
                rotation = tile.Rotation,
                hasMonasteryMeeple = tile.HasMonasteryMeeple,
                monasteryMeepleOwnerId = tile.HasMonasteryMeeple
                                           ? tile.MonasteryMeepleOwner.PlayerId
                                           : -1,
                segmentData = new List<SegmentSaveData>()
            };

            foreach (var seg in tile.GetSegments())
            {
                placed.segmentData.Add(new SegmentSaveData
                {
                    id = seg.Id,
                    hasMeeple = seg.HasMeeple,
                    meepleOwnerId = seg.HasMeeple
                                     ? seg.MeepleOwner.PlayerId
                                     : -1
                });
            }

            snapshot.placedTiles.Add(placed);
        }

        // 3) Зберігаємо гравців
        foreach (var player in playerManager.Players)
        {
            snapshot.players.Add(new PlayerSaveData
            {
                id = player.PlayerId,
                spriteIndex = player.MeepleSpriteIndex,
                meepleCount = player.MeepleCount,
                score = player.Score,
                name = player.Name
            });
        }

        // 4) Зберігаємо решту колоди
        foreach (var tileData in deckManager.fullDeck)
        {
            snapshot.remainingTiles.Add(tileData.name);
        }

        // 5) Серіалізуємо в JSON і пишемо на диск
        string json = JsonUtility.ToJson(snapshot, true);
        File.WriteAllText(GetSavePath(saveId), json);
        Debug.Log($"Гру збережено: {saveId}");
    }

    public static GameSnapshot LoadGame(string saveId)
    {
        string path = GetSavePath(saveId);
        if (!File.Exists(path))
        {
            Debug.LogError($"Збереження {saveId} не знайдено!");
            return null;
        }

        string json = File.ReadAllText(path);
        return JsonUtility.FromJson<GameSnapshot>(json);
    }

    public static List<string> GetAllSaveIds()
    {
        if (!Directory.Exists(saveFolder))
            return new List<string>();

        var files = Directory.GetFiles(saveFolder, "*.json");
        var list = new List<string>();
        foreach (var path in files)
            list.Add(Path.GetFileNameWithoutExtension(path));
        return list;
    }

    public static void DeleteSave(string saveId)
    {
        string path = GetSavePath(saveId);
        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log($"Збереження {saveId} видалено.");
        }
    }

    private static string GetSavePath(string saveId)
    {
        return Path.Combine(saveFolder, $"{saveId}.json");
    }
}