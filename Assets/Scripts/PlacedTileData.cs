using System;
using System.Collections.Generic;
using UnityEngine;

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
