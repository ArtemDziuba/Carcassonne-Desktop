using System;
using System.Collections.Generic;
using UnityEngine;

// Клас, що відповідає за збереження/завантаження гри безпосередньо на дошці
[Serializable]
public class GameSnapshot
{
    public List<PlacedTileData> placedTiles;
    public List<PlayerSaveData> players;
    public int currentPlayerIndex;
    public List<string> remainingTiles; // назви TileData
    public bool isFieldEnabled;
    public string saveName;
    public string date;
    public bool tilePlaced;
    public bool meeplePlaced;
}

