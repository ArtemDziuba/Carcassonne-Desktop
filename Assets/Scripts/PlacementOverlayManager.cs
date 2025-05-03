using System.Collections.Generic;
using UnityEngine;

public class PlacementOverlayManager : MonoBehaviour
{
    public PlacementTile placementTilePrefab;

    private List<GameObject> currentOverlays = new();

    /// <summary>
    /// Показує зелений оверлей на позиціях, де можна поставити тайл
    /// </summary>
    /// <param name="positions">Позиції на сітці, які доступні для розміщення</param>
    public void ShowAvailablePositions(IEnumerable<Vector2Int> positions)
    {
        Clear();

        foreach (var pos in positions)
        {
            var overlay = Instantiate(placementTilePrefab);
            overlay.GridPosition = pos;
            overlay.transform.position = new Vector3(pos.x, pos.y, -1); // нижче тайлів
            currentOverlays.Add(overlay.gameObject);
        }
    }

    /// <summary>
    /// Видаляє всі оверлеї з поля
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
