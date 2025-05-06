using UnityEngine;
using UnityEngine.InputSystem;

public class MeepleSpawner : MonoBehaviour
{
    public GameObject meeplePrefab;
    public Sprite[] meepleSprites; // 5 кольорів міплів
    public int currentPlayerIndex = 0;
    public float snapDistance = 0.3f; // відстань для "прилипання"
    public TurnManager turnManager; // перетягнути вручну в інспекторі
    public Board board;
    public PlayerManager playerManager;

    private GameObject currentMeeple;
    private Camera mainCamera;
    private bool isPlacing = false;
    private MeeplePlacementSlot hoveredSlot;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    public void StartPlacingMeeple()
    {
        if (isPlacing || meeplePrefab == null) return;

        currentMeeple = Instantiate(meeplePrefab);
        isPlacing = true;

        // Встановити правильний спрайт
        SpriteRenderer sr = currentMeeple.GetComponent<SpriteRenderer>();
        if (sr != null && meepleSprites.Length > currentPlayerIndex)
        {
            sr.sprite = meepleSprites[currentPlayerIndex];
        }
    }

    private void Update()
    {
        if (!isPlacing || currentMeeple == null) return;

        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = 0;

        hoveredSlot = FindClosestSlot(mouseWorldPos);

        if (hoveredSlot != null)
        {
            currentMeeple.transform.position = hoveredSlot.transform.position;
        }
        else
        {
            currentMeeple.transform.position = mouseWorldPos;
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            TryPlaceMeeple();
        }
    }

    private MeeplePlacementSlot FindClosestSlot(Vector3 pos)
    {
        MeeplePlacementSlot[] allSlots =
            Object.FindObjectsByType<MeeplePlacementSlot>(FindObjectsSortMode.None); // без сортування — швидше

        MeeplePlacementSlot closest = null;
        float minDist = float.MaxValue;

        foreach (var slot in allSlots)
        {
            if (slot.IsOccupied) continue;
            float dist = Vector3.Distance(slot.transform.position, pos);
            if (dist < snapDistance && dist < minDist)
            {
                closest = slot;
                minDist = dist;
            }
        }

        return closest;
    }

    private void TryPlaceMeeple()
    {
        if (hoveredSlot == null) return;

        Tile tile = hoveredSlot.transform.parent.GetComponent<Tile>();
        if (tile == null) return;

        Vector2Int tilePos = new Vector2Int(
            Mathf.RoundToInt(tile.transform.position.x),
            Mathf.RoundToInt(tile.transform.position.y)
        );

        // Перевірка: чи структура вже зайнята
        bool occupied = StructureAnalyzer.IsStructureOccupied(board, tilePos, hoveredSlot.CoveredSegments);
        if (occupied)
        {
            Debug.Log("[MeepleSpawner] Структура вже зайнята, міпл не може бути розміщений.");
            return;
        }

        // Прив'язка міпла до слота
        hoveredSlot.CurrentMeeple = currentMeeple;
        currentMeeple.transform.position = hoveredSlot.transform.position;
        isPlacing = false;
        currentMeeple = null;

        // Збереження стану міпла в сегментах
        foreach (int segId in hoveredSlot.CoveredSegments)
        {
            Segment seg = tile.GetSegments()[segId];
            seg.HasMeeple = true;
            seg.MeepleOwner = playerManager.GetCurrentPlayer();
        }

        // Прибираємо візуальне підсвічування
        var sr = hoveredSlot.GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.enabled = false;

        turnManager.OnMeeplePlaced();

        Debug.Log($"[MeepleSpawner] Міпл розміщено на слоті: {hoveredSlot.transform.position}");
    }


}
