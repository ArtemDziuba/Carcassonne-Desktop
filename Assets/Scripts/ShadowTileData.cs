using System.Collections.Generic;
using UnityEngine;

public class ShadowTileData
{
    public Vector2Int Position;
    public List<Segment> Segments;

    public ShadowTileData(Vector2Int position)
    {
        Position = position;
        Segments = new List<Segment>();

        // Ініціалізуємо всі 12 сегментів як null (явно)
        for (int i = 0; i < 12; i++)
            Segments.Add(null);
    }

    public void SetSegment(int index, TerrainType type)
    {
        if (index < 0 || index >= 12) return;

        Segments[index] = new Segment
        {
            Id = index,
            Type = type,
            ConnectedSegmentIds = new List<int>()
        };
    }

    public bool Matches(TileData tileData, int rotation)
    {
        List<Segment> rotated = new List<Segment>(tileData.GetRotatedSegments(rotation));

        for (int i = 0; i < 12; i++)
        {
            Segment shadowSeg = Segments[i];
            if (shadowSeg == null) continue; // невизначена вершина

            Segment tileSeg = rotated[i];
            if (shadowSeg.Type != tileSeg.Type)
                return false;
        }

        return true;
    }

}
