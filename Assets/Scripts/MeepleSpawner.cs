// MeepleSpawner.cs
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

// Клас, що відповідає за створення міплів безпосередньо у грі
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

    AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

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

        var currentPlayer = playerManager.GetCurrentPlayer();
        sr.sprite = meepleSprites[currentPlayer.MeepleSpriteIndex];

        isPlacing = true;
    }

    private void Update()
    {
        if (!isPlacing || currentMeeple == null) return;

        // Відміна розміщення ПКМ
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            audioManager.PlaySFX(audioManager.meepleTakeBack);

            Destroy(currentMeeple);
            currentMeeple = null;
            isPlacing = false;
            hoveredSlot = null;
            turnManager.endTurnBtn.interactable = true;
            turnManager.placeMeepleBtn.interactable = true;
            return;
        }

        Vector2 mouseScreen = Mouse.current.position.ReadValue();
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(mouseScreen);
        worldPos.z = 0f;

        // Пошук найближчого слоту
        hoveredSlot = Object
            .FindObjectsByType<MeeplePlacementSlot>(FindObjectsSortMode.None)
            .Where(s => !s.IsOccupied)
            .OrderBy(s => Vector3.Distance(s.transform.position, worldPos))
            .FirstOrDefault(s => Vector3.Distance(s.transform.position, worldPos) < snapDistance);

        // Переміщуємо міпла за курсором або на слот
        currentMeeple.transform.position = hoveredSlot != null
            ? hoveredSlot.transform.position
            : worldPos;

        // Підтвердження розміщення ЛКМ
        if (Mouse.current.leftButton.wasPressedThisFrame)
            TryPlaceMeeple();
    }

    private void TryPlaceMeeple()
    {
        if (hoveredSlot == null)
        {
            audioManager.PlaySFX(audioManager.reject);
            return;
        }
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
                ToastManager.Instance.ShowToast(ToastType.Warning, 
                    "Структура вже зайнята, міпл не може бути розміщений.");
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

        // Завершуємо фазу розміщення
        currentMeeple = null;
        isPlacing = false;
        hoveredSlot = null;
        turnManager.OnMeeplePlaced();
    }
}
