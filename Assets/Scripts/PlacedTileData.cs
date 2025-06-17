using System;
using System.Collections.Generic;
using UnityEngine;

// Клас, що відповідає за збереження інформації розміщених тайлів на дошці
[Serializable]
public class PlacedTileData
{
    public string tileName;
    public Vector2Int position;
    public int rotation;
    public bool hasMonasteryMeeple;
    public int monasteryMeepleOwnerId;

    public List<SegmentSaveData> segmentData = new();
}
