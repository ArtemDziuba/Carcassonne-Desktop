using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public TileData Data;
    public SpriteRenderer SpriteRenderer;

    public int Rotation = 0; // 0, 90, 180, 270

    private List<Segment> rotatedSegments;

    void Start()
    {
        if (!Application.isPlaying) return;

        if (Data == null || SpriteRenderer == null)
        {
            Debug.LogError("TileData or SpriteRenderer is missing.");
            return;
        }

        SpriteRenderer.sprite = Data.TileSprite;
        rotatedSegments = CloneSegments(Data.Segments);
        UpdateRotation();
    }

    public void RotateClockwise()
    {
        Rotation = (Rotation + 90) % 360;
        UpdateRotation();
        RotateSegments(90);
    }

    private void UpdateRotation()
    {
        transform.rotation = Quaternion.Euler(0f, 0f, -Rotation);
    }

    public void RotateSegments(int degrees)
    {
        int shift = 3 * (degrees / 90); // 3 сегменти на сторону
        var newSegments = new List<Segment>(new Segment[12]);

        for (int i = 0; i < 12; i++)
        {
            int newIndex = (i + shift) % 12;
            Segment oldSeg = rotatedSegments[i];

            Segment rotated = new Segment
            {
                Id = newIndex,
                Type = oldSeg.Type,
                HasMeeple = oldSeg.HasMeeple,
                MeepleOwner = oldSeg.MeepleOwner,
                ConnectedSegmentIds = new List<int>()
            };

            newSegments[newIndex] = rotated;
        }

        // Встановлюємо зв’язки
        for (int i = 0; i < 12; i++)
        {
            int oldIndex = (i - shift + 12) % 12;
            Segment oldSeg = rotatedSegments[oldIndex];
            Segment newSeg = newSegments[i];

            foreach (var oldConnectedId in oldSeg.ConnectedSegmentIds)
            {
                int newConnectedId = (oldConnectedId + shift) % 12;
                newSeg.ConnectedSegmentIds.Add(newConnectedId);
            }
        }

        rotatedSegments = newSegments;
    }

    private List<Segment> CloneSegments(List<Segment> source)
    {
        List<Segment> clone = new List<Segment>();
        foreach (var seg in source)
        {
            Segment copy = new Segment
            {
                Id = seg.Id,
                Type = seg.Type,
                HasMeeple = seg.HasMeeple,
                MeepleOwner = seg.MeepleOwner,
                ConnectedSegmentIds = new List<int>(seg.ConnectedSegmentIds)
            };
            clone.Add(copy);
        }
        return clone;
    }

    public List<Segment> GetSegments()
    {
        return rotatedSegments;
    }
}
