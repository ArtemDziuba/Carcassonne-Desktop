using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTileData", menuName = "Carcassonne/TileData")]
public class TileData : ScriptableObject
{
    public string TileId;
    public Sprite TileSprite;

    public List<Segment> Segments = new List<Segment>(); // повноцінні об’єкти
    public List<MeeplePlacementSlotData> MeepleSlots;

    public bool HasMonastery;
    public bool HasShield;
}
