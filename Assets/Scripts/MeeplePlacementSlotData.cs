using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MeeplePlacementSlotData
{
    public TerrainType Type;
    public List<int> CoveredSegmentIds;
}
