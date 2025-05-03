using System.Collections.Generic;
using UnityEngine;

public class PlacementOverlayManager : MonoBehaviour
{
    public PlacementTile placementTilePrefab;

    private List<GameObject> currentOverlays = new();

    /// <summary>
    /// ������ ������� ������� �� ��������, �� ����� ��������� ����
    /// </summary>
    /// <param name="positions">������� �� ����, �� ������� ��� ���������</param>
    public void ShowAvailablePositions(IEnumerable<Vector2Int> positions)
    {
        Clear();

        foreach (var pos in positions)
        {
            var overlay = Instantiate(placementTilePrefab);
            overlay.GridPosition = pos;
            overlay.transform.position = new Vector3(pos.x, pos.y, -1); // ����� �����
            currentOverlays.Add(overlay.gameObject);
        }
    }

    /// <summary>
    /// ������� �� ������ � ����
    /// </summary>
    public void Clear()
    {
        foreach (var obj in currentOverlays)
        {
            Destroy(obj);
        }
        currentOverlays.Clear();
    }
}
