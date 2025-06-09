using System.Collections.Generic;
using UnityEngine;

// Клас, що відповідає за утримання даних слотів міплів
[System.Serializable]
public class MeeplePlacementSlotData
{
    public TerrainType Type;
    public List<int> CoveredSegmentIds;
}
