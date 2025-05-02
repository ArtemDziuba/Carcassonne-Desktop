using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class TileSpawner : MonoBehaviour
{
    public Tile tilePrefab;
    public Board board;
    public TileDeckManager deck;
    public PlacementOverlayManager overlayManager;

    private Tile currentTile;
    private Camera mainCamera;
    private int tileZ = -5;

    private Vector2Int? snappedGridPos = null;
    private HashSet<Vector2Int> currentValidPositions = new();

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

        // 2. Пошук найближчої допустимої позиції
        snappedGridPos = null;
        float minDist = float.MaxValue;

        foreach (var pos in currentValidPositions)
        {
            float dist = Vector2.Distance(mouse2D, new Vector2(pos.x, pos.y));
            if (dist <= 1.0f && dist < minDist)
            {
                snappedGridPos = pos;
                minDist = dist;
            }
        }

        // 3. Прив?язка тайла
        if (snappedGridPos != null)
        {
            currentTile.transform.position = new Vector3(snappedGridPos.Value.x, snappedGridPos.Value.y, tileZ);
        }
        else
        {
            currentTile.transform.position = new Vector3(mouse2D.x, mouse2D.y, tileZ);
        }

        // 4. Поворот тайла
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            currentTile.RotateClockwise();

            currentValidPositions = GetValidPlacements(currentTile.Data);
            overlayManager.ShowAvailablePositions(currentValidPositions);
        }

        // 5. Розміщення тайла
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (snappedGridPos != null && board.CanPlaceTileAt(snappedGridPos.Value, currentTile.Data, currentTile.Rotation))
            {
                board.PlaceTile(snappedGridPos.Value, currentTile);
                overlayManager.Clear();
                currentValidPositions.Clear();
                currentTile = null;
            }
            else
            {
                Debug.Log("NOPE Неможливо поставити тайл у це місце.");
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

        currentValidPositions = GetValidPlacements(next);
        overlayManager.ShowAvailablePositions(currentValidPositions);
    }

    private HashSet<Vector2Int> GetValidPlacements(TileData tileData)
    {
        var valid = new HashSet<Vector2Int>();
        var candidates = board.GetValidPositions();

        for (int rot = 0; rot < 360; rot += 90)
        {
            foreach (var pos in candidates)
            {
                if (board.CanPlaceTileAt(pos, tileData, rot))
                    valid.Add(pos);
            }
        }

        return valid;
    }
}
