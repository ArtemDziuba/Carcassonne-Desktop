using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class MeepleSpawner : MonoBehaviour
{
    public GameObject meeplePrefab;
    public Sprite[] meepleSprites;
    public float snapDistance = 0.3f;
    public TurnManager turnManager;
    public Board board;
    public PlayerManager playerManager;

    private GameObject currentMeeple;
    private bool isPlacing;
    private MeeplePlacementSlot hoveredSlot;
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
        isPlacing = false;
    }

    public void StartPlacingMeeple()
    {
        if (isPlacing || meeplePrefab == null) return;

        currentMeeple = Instantiate(meeplePrefab);
        var sr = currentMeeple.GetComponent<SpriteRenderer>();
        sr.sprite = meepleSprites[playerManager.CurrentPlayerIndex];
        isPlacing = true;
    }

    private void Update()
    {
        if (!isPlacing || currentMeeple == null) return;

        Vector2 mouseScreen = Mouse.current.position.ReadValue();
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(mouseScreen);
        worldPos.z = 0f;

        hoveredSlot = Object
            .FindObjectsByType<MeeplePlacementSlot>(FindObjectsSortMode.None)
            .Where(s => !s.IsOccupied)
            .OrderBy(s => Vector3.Distance(s.transform.position, worldPos))
            .FirstOrDefault(s => Vector3.Distance(s.transform.position, worldPos) < snapDistance);

        currentMeeple.transform.position = hoveredSlot != null
            ? hoveredSlot.transform.position
            : worldPos;

        if (Mouse.current.leftButton.wasPressedThisFrame)
            TryPlaceMeeple();
    }

    private void TryPlaceMeeple()
    {
        if (hoveredSlot == null) return;

        var tile = hoveredSlot.GetComponentInParent<Tile>();
        var player = playerManager.GetCurrentPlayer();

        // 1) Монастир
        if (tile.Data.HasMonastery && hoveredSlot.Type == TerrainType.Monastery)
        {
            tile.PlaceMonasteryMeeple(player, currentMeeple);
            hoveredSlot.IsOccupied = true;
            hoveredSlot.CurrentMeeple = currentMeeple;
            hoveredSlot.MeepleOwner = player;
        }
        else
        {
            // 2) Дороги/міста/поля
            Vector2Int tilePos = new Vector2Int(
                Mathf.RoundToInt(tile.transform.position.x),
                Mathf.RoundToInt(tile.transform.position.y)
            );

            if (StructureAnalyzer.IsStructureOccupied(board, tilePos, hoveredSlot.CoveredSegments))
            {
                Debug.Log("[MeepleSpawner] Структура вже зайнята, міпл не може бути розміщений.");
                return;
            }

            foreach (int segId in hoveredSlot.CoveredSegments)
            {
                var seg = tile.GetSegments()[segId];
                seg.HasMeeple = true;
                seg.MeepleOwner = player;
                seg.MeepleObject = currentMeeple;
            }

            hoveredSlot.IsOccupied = true;
            hoveredSlot.CurrentMeeple = currentMeeple;
            hoveredSlot.MeepleOwner = player;
        }

        currentMeeple = null;
        isPlacing = false;
        turnManager.OnMeeplePlaced();
    }
}