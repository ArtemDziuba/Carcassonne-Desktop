using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

// Клас, що відповідає за реалізацію збережень
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

        var snapshot = new GameSnapshot
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

        // --- Tiles on board ---
        foreach (var pair in board.placedTiles)
        {
            var tile = pair.Value;
            var data = new PlacedTileData
            {
                tileName = tile.Data.name,
                position = pair.Key,
                rotation = tile.Rotation,
                hasMonasteryMeeple = tile.HasMonasteryMeeple,
                monasteryMeepleOwnerId = tile.HasMonasteryMeeple
                                        ? tile.MonasteryMeepleOwner.PlayerId
                                        : -1
            };

            // Серіалізуємо **тільки** сегменти з міплами
            foreach (var seg in tile.GetSegments())
            {
                if (!seg.HasMeeple)
                    continue;

                data.segmentData.Add(new SegmentSaveData
                {
                    id = seg.Id,
                    hasMeeple = true,
                    meepleOwnerId = seg.MeepleOwner != null
                                    ? seg.MeepleOwner.PlayerId
                                    : -1
                });
            }

            snapshot.placedTiles.Add(data);
        }

        // --- Players ---
        foreach (var pl in playerManager.Players)
        {
            snapshot.players.Add(new PlayerSaveData
            {
                id = pl.PlayerId,
                spriteIndex = pl.MeepleSpriteIndex,
                meepleCount = pl.MeepleCount,
                score = pl.Score,
                name = pl.Name
            });
        }

        // --- Deck ---
        foreach (var tileData in deckManager.fullDeck)
            snapshot.remainingTiles.Add(tileData.name);

        var json = JsonUtility.ToJson(snapshot, true);

        AES crypto = new AES();
        byte[]encrytpetJson = crypto.Encrypt(json);

        //File.WriteAllText(saveId+".json", json);
        File.WriteAllBytes(GetSavePath(saveId), encrytpetJson);
        Debug.Log($"[SAVE] Гру збережено: {saveId} ({snapshot.placedTiles.Count} плиток)");
    }

    public static GameSnapshot LoadGame(string saveId)
    {
        string path = GetSavePath(saveId);
        if (!File.Exists(path))
        {
            Debug.LogError($"Збереження {saveId} не знайдено!");
            return null;
        }

        byte[] encrytpetJson = File.ReadAllBytes(path);

        AES crypto = new AES();
        string json = crypto.Decrypt(encrytpetJson);

        return JsonUtility.FromJson<GameSnapshot>(json);
    }

    public static List<string> GetAllSaveIds()
    {
        if (!Directory.Exists(saveFolder))
            return new List<string>();

        var files = Directory.GetFiles(saveFolder, "*.dat");
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
        return Path.Combine(saveFolder, $"{saveId}.dat");
    }
}