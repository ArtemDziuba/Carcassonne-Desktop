using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class PlacedTileData
{
    public string tileName;     // Назва або ID TileData (назва .asset або Resources path)
    public Vector2Int position;
    public int rotation;
    public bool hasMonasteryMeeple;
    public int monasteryMeepleOwnerId;
    public List<SegmentSaveData> segmentData;
}