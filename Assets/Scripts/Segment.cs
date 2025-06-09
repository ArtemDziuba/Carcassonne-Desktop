using System.Collections.Generic;
using UnityEngine;

// Клас, що відповідає за сегмент тайлу
[System.Serializable]
public class Segment
{
    public int Id; // Локальний ID в межах тайлу
    public TerrainType Type;
    public List<int> ConnectedSegmentIds = new List<int>();

    public bool HasMeeple = false;
    public Player MeepleOwner = null;

    [HideInInspector] public GameObject MeepleObject;
}
