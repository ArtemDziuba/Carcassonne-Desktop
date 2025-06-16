using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

// Клас, що відповідає за створення та розміщення тайлів безпосередньо у грі
public class TileSpawner : MonoBehaviour
{
    public Tile tilePrefab;
    public Board board;
    public TileDeckManager deck;
    public ShadowOverlayManager overlayManager;
    public TurnManager turnManager;

    private Tile currentTile;
    private Camera mainCamera;
    private int tileZ = -5;
    private Vector2Int? snappedGridPos = null;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (currentTile == null) return;

        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(mouseScreenPos);
        Vector2 mouse2D = new Vector2(mouseWorld.x, mouseWorld.y);

        snappedGridPos = null;
        float minDist = float.MaxValue;

        foreach (var pos in board.GetAllShadowPositions())
        {
            if (board.GetTileAt(pos)) continue;

            ShadowTileData shadow = board.GetShadowAt(pos);
            if (shadow != null && shadow.Matches(currentTile.Data, currentTile.Rotation))
            {
                float dist = Vector2.Distance(mouse2D, pos);
                if (dist < 1.0f && dist < minDist)
                {
                    snappedGridPos = pos;
                    minDist = dist;
                }
            }
        }

        if (snappedGridPos.HasValue)
            currentTile.transform.position = new Vector3(snappedGridPos.Value.x, snappedGridPos.Value.y, tileZ);
        else
            currentTile.transform.position = new Vector3(mouse2D.x, mouse2D.y, tileZ);

        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            currentTile.RotateClockwise();
            ShowMatchingShadowTiles();
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (snappedGridPos.HasValue)
            {
                board.PlaceTile(snappedGridPos.Value, currentTile);
                overlayManager.Clear();
                currentTile = null;
                turnManager.OnTilePlaced();
            }
            else
            {
                ToastManager.Instance.ShowToast(ToastType.Warning,
                "Неможливо поставити тайл: невірна позиція.");
                Debug.Log("Неможливо поставити тайл: невірна позиція.");
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
        currentTile.Initialize(next);
        currentTile.transform.position = new Vector3(0, 0, tileZ);

        ShowMatchingShadowTiles();
    }

    private void ShowMatchingShadowTiles()
    {
        var positions = new List<Vector2Int>();

        foreach (var pos in board.GetAllShadowPositions())
        {
            if (board.GetTileAt(pos)) continue;

            var shadow = board.GetShadowAt(pos);
            if (shadow != null && shadow.Matches(currentTile.Data, currentTile.Rotation))
                positions.Add(pos);
        }

        overlayManager.ShowAvailablePositions(positions);
    }
}
