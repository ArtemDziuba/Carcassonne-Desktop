using System.Collections.Generic;
using UnityEngine;

//  лас, що в≥дпов≥даЇ за дан≥ та лог≥ку тайлу п≥дсв≥тки розм≥щенн€
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
            Segments[index].Type = type; // оновлюЇмо тип, €кщо вже Ї
        }
    }

    public bool Matches(TileData tileData, int rotation)
    {
        Segment[] rotated = tileData.GetRotatedSegments(rotation);

        // ≤ндекси стор≥н: top, right, bottom, left
        int[][] tileSideIndices = new int[][]
        {
        new int[] { 0, 1, 2 },    // низ Ч це верх shadow
        new int[] { 3, 4, 5 },    // л≥во Ч це право shadow
        new int[] { 6, 7, 8 },    // верх Ч це низ shadow
        new int[] { 9, 10, 11 }   // право Ч це л≥во shadow
        };

        int[][] shadowSideIndices = new int[][]
        {
        new int[] { 0, 1, 2 },    // низ Ч це верх shadow
        new int[] { 3, 4, 5 },    // л≥во Ч це право shadow
        new int[] { 6, 7, 8 },    // верх Ч це низ shadow
        new int[] { 9, 10, 11 }   // право Ч це л≥во shadow
        };

        for (int side = 0; side < 4; side++)
        {
            bool isDefined = false;

            // перев≥р€Їмо, чи Ї хоч один визначений сегмент з ц≥Їњ сторони в shadow
            foreach (int i in shadowSideIndices[side])
            {
                if (Segments[i] != null)
                {
                    isDefined = true;
                    break;
                }
            }

            // €кщо Ї Ч перев≥р€Їмо повний зб≥г з в≥дпов≥дною стороною tile
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
