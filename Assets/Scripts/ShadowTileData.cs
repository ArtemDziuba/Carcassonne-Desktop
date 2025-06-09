using System.Collections.Generic;
using UnityEngine;

// ����, �� ������� �� ��� �� ����� ����� ������� ���������
public class ShadowTileData
{
    public Vector2Int Position;
    public List<Segment> Segments;

    public ShadowTileData(Vector2Int position)
    {
        Position = position;
        Segments = new List<Segment>();

        for (int i = 0; i < 12; i++)
            Segments.Add(null);
    }

    public void SetSegment(int index, TerrainType type)
    {
        if (index < 0 || index >= 12) return;

        if (Segments[index] == null)
        {
            Segments[index] = new Segment
            {
                Id = index,
                Type = type,
                ConnectedSegmentIds = new List<int>()
            };
        }
        else
        {
            Segments[index].Type = type; // ��������� ���, ���� ��� �
        }
    }

    public bool Matches(TileData tileData, int rotation)
    {
        Segment[] rotated = tileData.GetRotatedSegments(rotation);

        // ������� �����: top, right, bottom, left
        int[][] tileSideIndices = new int[][]
        {
        new int[] { 0, 1, 2 },    // ��� � �� ���� shadow
        new int[] { 3, 4, 5 },    // ��� � �� ����� shadow
        new int[] { 6, 7, 8 },    // ���� � �� ��� shadow
        new int[] { 9, 10, 11 }   // ����� � �� ��� shadow
        };

        int[][] shadowSideIndices = new int[][]
        {
        new int[] { 0, 1, 2 },    // ��� � �� ���� shadow
        new int[] { 3, 4, 5 },    // ��� � �� ����� shadow
        new int[] { 6, 7, 8 },    // ���� � �� ��� shadow
        new int[] { 9, 10, 11 }   // ����� � �� ��� shadow
        };

        for (int side = 0; side < 4; side++)
        {
            bool isDefined = false;

            // ����������, �� � ��� ���� ���������� ������� � ���� ������� � shadow
            foreach (int i in shadowSideIndices[side])
            {
                if (Segments[i] != null)
                {
                    isDefined = true;
                    break;
                }
            }

            // ���� � � ���������� ������ ��� � ��������� �������� tile
            if (isDefined)
            {
                for (int i = 0; i < 3; i++)
                {
                    Segment shadowSeg = Segments[shadowSideIndices[side][i]];
                    Segment tileSeg = rotated[tileSideIndices[side][i]];

                    if (shadowSeg == null) continue;
                    if (tileSeg == null || tileSeg.Type != shadowSeg.Type)
                        return false;
                }
            }
        }

        return true;
    }

}
