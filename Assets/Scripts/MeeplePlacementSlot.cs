using System.Collections.Generic;
using UnityEngine;

public class MeeplePlacementSlot : MonoBehaviour
{
    public TerrainType Type;
    public List<int> CoveredSegments;

    [HideInInspector] public bool IsOccupied = false;
    [HideInInspector] public GameObject CurrentMeeple;

    private void OnDrawGizmos()
    {
        Gizmos.color = IsOccupied ? Color.red : Color.cyan;

        // ���� � BoxCollider2D � ������� �����������
        var collider = GetComponent<BoxCollider2D>();
        if (collider != null)
        {
            Gizmos.DrawWireCube(transform.position, collider.size);
        }
        else
        {
            Gizmos.DrawSphere(transform.position, 0.1f);
        }
    }
}
