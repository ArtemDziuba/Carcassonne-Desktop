using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public TileData Data { get; private set; }
    public SpriteRenderer SpriteRenderer { get; private set; }
    public int Rotation { get; private set; }

    // Для монастиря
    private bool hasMonasteryMeeple = false;
    public bool HasMonasteryMeeple => hasMonasteryMeeple;
    private Player monasteryMeepleOwner;
    public Player MonasteryMeepleOwner => monasteryMeepleOwner;
    private GameObject monasteryMeepleObject;

    // Сегменти для доріг/міст/полів
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
        CreateMeepleSlots();
    }

    private void UpdateRotation()
    {
        transform.rotation = Quaternion.Euler(0f, 0f, -Rotation);
    }

    private void RotateSegments(int degrees)
    {
        int shift = (degrees / 90) * 3;
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
            foreach (int conn in rotatedSegments[oldIndex].ConnectedSegmentIds)
                newSegments[i].ConnectedSegmentIds.Add((conn + shift) % 12);
        }
        rotatedSegments = newSegments;
    }

    private List<Segment> CloneSegments(List<Segment> source)
    {
        var clone = new List<Segment>(source.Count);
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

    // Розміщення та очищення міпла на монастирі
    public void PlaceMonasteryMeeple(Player owner, GameObject meepleObj)
    {
        if (!Data.HasMonastery) return;
        hasMonasteryMeeple = true;
        monasteryMeepleOwner = owner;
        monasteryMeepleObject = meepleObj;
    }

    public void ClearMonasteryMeeple()
    {
        if (monasteryMeepleObject != null)
            Destroy(monasteryMeepleObject);
        hasMonasteryMeeple = false;
        monasteryMeepleOwner = null;
        monasteryMeepleObject = null;
    }

    // Створення слотів для міплів на сегментах
    public void CreateMeepleSlots()
    {
        ClearMeepleSlots();
        int shift = (Rotation / 90) * 3;
        foreach (var slotData in Data.MeepleSlots)
        {
            GameObject slotObj = new GameObject("MeepleSlot");
            slotObj.transform.SetParent(transform);
            slotObj.transform.localPosition = CalculateSlotPosition(slotData);

            var slot = slotObj.AddComponent<MeeplePlacementSlot>();
            slot.Type = slotData.Type;
            slot.CoveredSegments = new List<int>();
            foreach (int originalId in slotData.CoveredSegmentIds)
                slot.CoveredSegments.Add((originalId + shift) % 12);

            var collider = slotObj.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
            collider.size = new Vector2(0.2f, 0.2f);
        }
    }

    public void ClearMeepleSlots()
    {
        foreach (Transform child in transform)
            if (child.GetComponent<MeeplePlacementSlot>())
                Destroy(child.gameObject);
    }

    private Vector3 CalculateSlotPosition(MeeplePlacementSlotData slotData)
    {
        if (slotData.CoveredSegmentIds == null || slotData.CoveredSegmentIds.Count == 0)
            return Vector3.zero;

        if (slotData.CoveredSegmentIds.Count == 2 &&
            Mathf.Abs(slotData.CoveredSegmentIds[0] - slotData.CoveredSegmentIds[1]) == 3)
        {
            int id = slotData.CoveredSegmentIds[1];
            Vector3 pos = GetSegmentLocalPosition(id);
            return new Vector3(pos.x, pos.y + 0.1f, 0f);
        }

        if (slotData.CoveredSegmentIds.Count >= 11)
        {
            int id = slotData.CoveredSegmentIds[0];
            Vector3 pos = GetSegmentLocalPosition(id);
            return new Vector3(pos.x, pos.y * 0.7f, 0f);
        }

        Vector3 sum = Vector3.zero;
        foreach (int id in slotData.CoveredSegmentIds)
            sum += GetSegmentLocalPosition(id);

        Vector3 avg = sum / slotData.CoveredSegmentIds.Count;
        avg.x *= -1;
        return avg;
    }

    private Vector3 GetSegmentLocalPosition(int segmentId)
    {
        float radius = 0.5f;
        float angleDeg = (segmentId / 12f) * 360f + 60f;
        float rad = angleDeg * Mathf.Deg2Rad;
        return new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f) * radius;
    }
}