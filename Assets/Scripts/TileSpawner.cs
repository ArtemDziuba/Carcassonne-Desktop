using UnityEngine;
using UnityEngine.InputSystem;

public class TileSpawner : MonoBehaviour
{
    public Tile tilePrefab;
    public Board board;
    public TileDeckManager deck;

    private Tile currentTile;
    private Camera mainCamera;
    private int tileZ = -5;

    private const float tileWorldSize = 1.0f; // бо 110 px / 100 ppu = 1.1f, без додаткового масштабу
    private const float halfTile = tileWorldSize / 2.0f;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (currentTile == null) return;

        // 1. Позиція миші у world-space
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(mouseScreenPos);

        // 2. Визначаємо логічну позицію на сітці
        Vector2Int gridPos = new Vector2Int(
            Mathf.RoundToInt(mouseWorld.x / tileWorldSize),
            Mathf.RoundToInt(mouseWorld.y / tileWorldSize)
        );

        // 3. Додаємо зміщення на пів тайла ВИЩЕ і ПРАВІШЕ (тобто +1 до логічної координати)
        Vector2Int shiftedGridPos = gridPos + Vector2Int.one;

        // 4. Візуальна позиція
        Vector3 snappedWorldPos = new Vector3(
            shiftedGridPos.x * tileWorldSize + halfTile,
            shiftedGridPos.y * tileWorldSize + halfTile,
            tileZ
        );

        // 4. Переміщення тайла
        currentTile.transform.position = snappedWorldPos;

        // 5. Поворот — права кнопка миші
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            currentTile.RotateClockwise();
        }

        // 6. Спроба розміщення — ліва кнопка миші
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (board.CanPlaceTileAt(gridPos, currentTile.Data, currentTile.Rotation))
            {
                currentTile.transform.position = new Vector3(
                    gridPos.x * tileWorldSize + halfTile,
                    gridPos.y * tileWorldSize + halfTile,
                    0 // після розміщення — на площину
                );

                board.PlaceTile(gridPos, currentTile);
                currentTile = null;
            }
            else
            {
                Debug.Log("NOPE Неприпустиме місце для тайла.");
            }
        }
    }

    public void SpawnNextTile()
    {
        if (currentTile != null)
        {
            Destroy(currentTile.gameObject);
        }

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
