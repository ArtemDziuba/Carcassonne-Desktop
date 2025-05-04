using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class MeeplePlacementSlot : MonoBehaviour
{
    public TerrainType Type;
    public List<int> CoveredSegments;

    public bool IsOccupied = false;
    [HideInInspector] public GameObject CurrentMeeple;

    private SpriteRenderer highlightRenderer;

    private void Awake()
    {
        // ��������� �������� ��'��� ��� �����������
        GameObject visual = new GameObject("Highlight");
        visual.transform.SetParent(transform);
        visual.transform.localPosition = Vector3.zero;
        visual.transform.localRotation = Quaternion.identity;
        visual.transform.localScale = new Vector3(0.15f, 0.15f, 1f); // ������� ������

        highlightRenderer = visual.AddComponent<SpriteRenderer>();
        highlightRenderer.sprite = Resources.Load<Sprite>("Sprites/Textures/meeplePlacementPoint"); // ���������� ����
        highlightRenderer.color = new Color(1f, 1f, 1f, 0.8f); // �������� ����
        highlightRenderer.sortingOrder = 2; // ������ �����
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position, 0.1f);
    }
}
