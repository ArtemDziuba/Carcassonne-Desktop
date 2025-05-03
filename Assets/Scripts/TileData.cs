using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileData", menuName = "Carcassonne/Tile Data")]
public class TileData : ScriptableObject
{
    public Sprite TileSprite;
    public List<Segment> Segments;

    public Segment[] GetRotatedSegments(int rotation)
    {
        int shift = 3 * (rotation / 90);
        Segment[] rotated = new Segment[12];

        for (int i = 0; i < 12; i++)
        {
            int newIndex = (i + shift) % 12;
            Segment src = Segments[i];
            rotated[newIndex] = new Segment
            {
                Id = newIndex,
                Type = src.Type,
                HasMeeple = false,
                MeepleOwner = null,
                ConnectedSegmentIds = new List<int>()
            };
        }

        for (int i = 0; i < 12; i++)
        {
            int oldIndex = (i - shift + 12) % 12;
            foreach (int oldConn in Segments[oldIndex].ConnectedSegmentIds)
            {
                int newConn = (oldConn + shift) % 12;
                rotated[i].ConnectedSegmentIds.Add(newConn);
            }
        }

        return rotated;
    }
}
