using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class TileSpawner : MonoBehaviour
{
    public Tile tilePrefab;
    public Board board;
    public TileDeckManager deck;

    private Tile currentTile;
    private Camera mainCamera;
    private int tileZ = -5;

    private const float snapDistance = 1.0f; // Дозволена відстань до "примагнічування"

    private Vector2Int? snappedGridPos = null;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (currentTile == null) return;

        // 1. Позиція миші
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(mouseScreenPos);
        Vector2 mouse2D = new Vector2(mouseWorld.x, mouseWorld.y);

        // 2. Отримати всі допустимі позиції
        HashSet<Vector2Int> validPositions = board.GetValidPositions();

        // 3. Знайти найближчу позицію в межах snapDistance
        snappedGridPos = null;
        float minDist = float.MaxValue;

        foreach (var pos in validPositions)
        {
            Vector2 worldPos = new Vector2(pos.x, pos.y);
            float dist = Vector2.Distance(mouse2D, worldPos);
            if (dist <= snapDistance && dist < minDist)
            {
                snappedGridPos = pos;
                minDist = dist;
            }
        }

        // 4. Якщо примагнічено — переміщаємо тайл до цієї позиції
        if (snappedGridPos != null)
        {
            currentTile.transform.position = new Vector3(snappedGridPos.Value.x, snappedGridPos.Value.y, tileZ);
        }
        else
        {
            // Інакше слідуємо точно за курсором
            currentTile.transform.position = new Vector3(mouse2D.x, mouse2D.y, tileZ);
        }

        // 5. Поворот
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            currentTile.RotateClockwise();
        }

        // 6. Спроба розмістити
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (snappedGridPos != null && board.CanPlaceTileAt(snappedGridPos.Value, currentTile.Data, currentTile.Rotation))
            {
                board.PlaceTile(snappedGridPos.Value, currentTile);
                currentTile = null;
            }
            else
            {
                Debug.Log("Nope Неможливо поставити тайл у це місце.");
            }
        }
    }

    public void SpawnNextTile()
    {
        if (currentTile != null)
            Destroy(currentTile.gameObject);

        TileData next = deck.DrawTile();
        if (next == null)
        {
            Debug.Log("Колода порожня.");
            return;
        }

        currentTile = Instantiate(tilePrefab);
        currentTile.Data = next;
        currentTile.SpriteRenderer = currentTile.GetComponent<SpriteRenderer>();
        currentTile.SpriteRenderer.sprite = next.TileSprite;

        currentTile.transform.position = new Vector3(0, 0, tileZ);
    }
}
