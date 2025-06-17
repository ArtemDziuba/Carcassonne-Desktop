using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Клас, що відповідає за розподіл ходів безпосередньо у грі
public class TurnManager : MonoBehaviour
{
    [Header("Managers & Spawners")]
    public PlayerManager playerManager;
    public TileSpawner tileSpawner;
    public MeepleSpawner meepleSpawner;
    public TileDeckManager deckManager;
    public Board board;
    public PlayerUIManager uiManager;

    [Header("UI Elements")]
    public Button chooseTileBtn;
    public Button placeMeepleBtn;
    public Button endTurnBtn;
    public Button unstuckBtn;
    public TextMeshProUGUI chooseTileBtnText;

    public bool tilePlaced = false;
    public bool meeplePlaced = false;

    AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    void Start()
    {
        if (GameConfig.Instance != null)
        {
            playerManager.SetPlayers(GameConfig.Instance.Players);
            uiManager.InitializeUI(playerManager.Players);
        }
        else
        {
            ToastManager.Instance.ShowToast(ToastType.Error,
                "Щось пішло не так.\nGameConfig.Instance не ініціалізовано.");
            Debug.LogError("GameConfig.Instance не ініціалізовано.");
            return;
        }

        chooseTileBtn.onClick.AddListener(OnChooseTile);
        placeMeepleBtn.onClick.AddListener(OnPlaceMeeple);
        endTurnBtn.onClick.AddListener(OnEndTurn);
        unstuckBtn.onClick.AddListener(OnUnstuck);
        NextTurnSetup();
    }        

    private void OnUnstuck()
    {
        tileSpawner.SwapTileToDeck();

        unstuckBtn.gameObject.SetActive(false);
    }

    /// <summary>
    /// Підготовка до нового ходу.
    /// </summary>
    private void NextTurnSetup()
    {
        // Якщо колода порожня одразу до початку ходу, гра завершується
        if (deckManager.IsEmpty())
        {
            EndGame();
            return;
        }

        tilePlaced = false;
        meeplePlaced = false;

        var current = playerManager.GetCurrentPlayer();
        uiManager.SetActivePlayer(playerManager.CurrentPlayerIndex);

        chooseTileBtn.interactable = true;
        placeMeepleBtn.interactable = false;
        endTurnBtn.interactable = false;


        // Оновлюємо лічильник тайлів на кнопці
        chooseTileBtnText.text = deckManager.TilesRemaining.ToString();

        // Прибираємо слоти підстав mіплів
        board.ClearAllMeepleSlotsExcept(null);

        ToastManager.Instance.ShowToast(ToastType.Info,
                "Затиснути ПКМ - для пересування по полю.", 5f);
        ToastManager.Instance.ShowToast(ToastType.Info,
                "Колесико мишки - для зміни масштабу", 5f);
    }

    /// <summary>
    /// Гравець вибирає наступний тайл.
    /// </summary>
    private void OnChooseTile()
    {
        audioManager.PlaySFX(audioManager.buttonClick);
        // Витягаємо та спавнимо тайл
        chooseTileBtn.interactable = false;
        tileSpawner.SpawnNextTile();

        ToastManager.Instance.ShowToast(ToastType.Info,
                "ПКМ - для обертання тайлу.", 5f);        
    }

    public void OnTilePlaced()
    {
        audioManager.PlaySFX(audioManager.tilePlaced);
        tilePlaced = true;

        // Оновлюємо кнопки
        var current = playerManager.GetCurrentPlayer();
        placeMeepleBtn.interactable = current.HasMeeples();
        endTurnBtn.interactable = true;
        unstuckBtn.gameObject.SetActive(false);
        chooseTileBtnText.text = deckManager.TilesRemaining.ToString();
    }

    /// <summary>
    /// Гравець переходить до розміщення міпла.
    /// </summary>
    private void OnPlaceMeeple()
    {
        audioManager.PlaySFX(audioManager.meeplePlace);
        var current = playerManager.GetCurrentPlayer();
        if (!current.HasMeeples())
        {
            Debug.LogWarning($"[TurnManager] Player {current.PlayerId} has no meeples left.");
            return;
        }

        ToastManager.Instance.ShowToast(ToastType.Info,
                "ПКМ - для скасування.", 5f);

        placeMeepleBtn.interactable = false;
        endTurnBtn.interactable = false;
        meepleSpawner.StartPlacingMeeple();
    }

    /// <summary>
    /// Гравець натиснув «End Turn».
    /// </summary>
    private void OnEndTurn()
    {
        if (!tilePlaced) return;

        audioManager.PlaySFX(audioManager.buttonClick);

        // Підрахунок очок за щойно завершені структури
        StructureScorer.ScoreCompletedStructures(board, playerManager, uiManager);

        // Перевіряємо кінець гри
        if (deckManager.IsEmpty())
        {
            EndGame();
            return;
        }

        // Повертаємо хід наступному гравцю
        playerManager.NextPlayer();
        NextTurnSetup();
    }

    /// <summary>
    /// Викликається MeepleSpawner, коли міпл успішно поставлений.
    /// </summary>
    public void OnMeeplePlaced()
    {
        audioManager.PlaySFX(audioManager.tilePlaced);

        meeplePlaced = true;
        endTurnBtn.interactable = true;

        var current = playerManager.GetCurrentPlayer();
        current.UseMeeple();
        uiManager.UpdatePlayerMeeples(playerManager.CurrentPlayerIndex, current.MeepleCount);

        // Прибираємо підсвічування та будь-які слоти
        board.ClearAllMeepleSlotsExcept(null);
    }

    /// <summary>
    /// Логіка завершення гри.
    /// </summary>
    private void EndGame()
    {
        // Рахуємо очки за всі незакриті структури
        StructureScorer.ScoreEndGameStructures(board, playerManager, uiManager);

        // Блокуємо всі кнопки
        chooseTileBtn.interactable = false;
        placeMeepleBtn.interactable = false;
        endTurnBtn.interactable = false;

        // Показуємо екран «Гра завершена»
        uiManager.ShowGameOverUI();
    }

    public void RestorePhase(bool tilePlaced, bool meeplePlaced)
    {
        this.tilePlaced = tilePlaced;
        this.meeplePlaced = meeplePlaced;

        chooseTileBtn.interactable = !tilePlaced;
        placeMeepleBtn.interactable = tilePlaced && !meeplePlaced;
        endTurnBtn.interactable = tilePlaced;
    }

    public void UpdateDeckCountUI()
    {
        chooseTileBtnText.text = deckManager.TilesRemaining.ToString();
    }
}
