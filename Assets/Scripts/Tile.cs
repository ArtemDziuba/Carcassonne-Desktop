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
        CreateMeepleSlots();
    }

    public void RotateClockwise()
    {
        Rotation = (Rotation + 90) % 360;
        RotateSegments(90);
        UpdateRotation();
        CreateMeepleSlots(); // пересоздаємо слоти після повороту
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

    public void CreateMeepleSlots()
    {
        // Видаляємо старі слоти (якщо були)
        foreach (Transform child in transform)
        {
            if (child.GetComponent<MeeplePlacementSlot>())
                Destroy(child.gameObject);
        }

        foreach (var slotData in Data.MeepleSlots)
        {
            GameObject slotObj = new GameObject("MeepleSlot");
            slotObj.transform.SetParent(this.transform);
            slotObj.transform.localPosition = CalculateSlotPosition(slotData);

            Debug.Log($"Створено слот на позиції: {slotObj.transform.localPosition}");

            var slot = slotObj.AddComponent<MeeplePlacementSlot>();
            slot.Type = slotData.Type;
            slot.CoveredSegments = new List<int>(slotData.CoveredSegmentIds);

            // Додаємо BoxCollider2D для взаємодії з мишею
            var collider = slotObj.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
            collider.size = new Vector2(0.2f, 0.2f); // Адаптуй за потреби
        }
    }


    private Vector3 CalculateSlotPosition(MeeplePlacementSlotData slotData)
    {
        int centerId = slotData.CoveredSegmentIds[slotData.CoveredSegmentIds.Count / 2];
        return GetSegmentLocalPosition(centerId);
    }

    private Vector3 GetSegmentLocalPosition(int segmentId)
    {
        float radius = 0.3f;
        float angle = ((segmentId + Rotation / 30f) / 12f) * 360f;
        float rad = angle * Mathf.Deg2Rad;
        return new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0) * radius;
    }
}
