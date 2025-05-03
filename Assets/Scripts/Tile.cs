using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public TileData Data { get; private set; }
    public SpriteRenderer SpriteRenderer { get; private set; }
    public int Rotation { get; private set; } = 0;

    private List<Segment> rotatedSegments;

    public void Initialize(TileData data)
    {
        Data = data;
        SpriteRenderer = GetComponent<SpriteRenderer>();

        if (Data == null || SpriteRenderer == null)
        {
            Debug.LogError("Tile initialization error: Data or SpriteRenderer is missing.");
            return;
        }

        SpriteRenderer.sprite = Data.TileSprite;
        rotatedSegments = CloneSegments(Data.Segments);
        Rotation = 0;
        UpdateRotation();
    }

    public void RotateClockwise()
    {
        Rotation = (Rotation + 90) % 360;
        RotateSegments(90);
        UpdateRotation();
    }

    private void UpdateRotation()
    {
        transform.rotation = Quaternion.Euler(0f, 0f, -Rotation);
    }

    private void RotateSegments(int degrees)
    {
        int shift = 3 * (degrees / 90);
        var newSegments = new List<Segment>(new Segment[12]);

        for (int i = 0; i < 12; i++)
        {
            int newIndex = (i + shift) % 12;
            Segment oldSeg = rotatedSegments[i];
            newSegments[newIndex] = new Segment
            {
                Id = newIndex,
                Type = oldSeg.Type,
                HasMeeple = oldSeg.HasMeeple,
                MeepleOwner = oldSeg.MeepleOwner,
                ConnectedSegmentIds = new List<int>()
            };
        }

        for (int i = 0; i < 12; i++)
        {
            int oldIndex = (i - shift + 12) % 12;
            foreach (int oldConn in rotatedSegments[oldIndex].ConnectedSegmentIds)
            {
                newSegments[i].ConnectedSegmentIds.Add((oldConn + shift) % 12);
            }
        }

        rotatedSegments = newSegments;
    }

    private List<Segment> CloneSegments(List<Segment> source)
    {
        var clone = new List<Segment>();
        foreach (var seg in source)
        {
            clone.Add(new Segment
            {
                Id = seg.Id,
                Type = seg.Type,
                HasMeeple = seg.HasMeeple,
                MeepleOwner = seg.MeepleOwner,
                ConnectedSegmentIds = new List<int>(seg.ConnectedSegmentIds)
            });
        }
        return clone;
    }

    public List<Segment> GetSegments()
    {
        return rotatedSegments;
    }
}
