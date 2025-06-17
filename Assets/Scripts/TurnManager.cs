using UnityEngine;
using UnityEngine.UI;
using TMPro;

// ����, �� ������� �� ������� ���� ������������� � ��
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
                "���� ���� �� ���.\nGameConfig.Instance �� ������������.");
            Debug.LogError("GameConfig.Instance �� ������������.");
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
    /// ϳ�������� �� ������ ����.
    /// </summary>
    private void NextTurnSetup()
    {
        // ���� ������ ������� ������ �� ������� ����, ��� �����������
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


        // ��������� �������� ����� �� ������
        chooseTileBtnText.text = deckManager.TilesRemaining.ToString();

        // ��������� ����� ������ m����
        board.ClearAllMeepleSlotsExcept(null);

        ToastManager.Instance.ShowToast(ToastType.Info,
                "��������� ��� - ��� ����������� �� ����.", 5f);
        ToastManager.Instance.ShowToast(ToastType.Info,
                "�������� ����� - ��� ���� ��������", 5f);
    }

    /// <summary>
    /// ������� ������ ��������� ����.
    /// </summary>
    private void OnChooseTile()
    {
        audioManager.PlaySFX(audioManager.buttonClick);
        // �������� �� �������� ����
        chooseTileBtn.interactable = false;
        tileSpawner.SpawnNextTile();

        ToastManager.Instance.ShowToast(ToastType.Info,
                "��� - ��� ��������� �����.", 5f);        
    }

    public void OnTilePlaced()
    {
        audioManager.PlaySFX(audioManager.tilePlaced);
        tilePlaced = true;

        // ��������� ������
        var current = playerManager.GetCurrentPlayer();
        placeMeepleBtn.interactable = current.HasMeeples();
        endTurnBtn.interactable = true;
        unstuckBtn.gameObject.SetActive(false);
        chooseTileBtnText.text = deckManager.TilesRemaining.ToString();
    }

    /// <summary>
    /// ������� ���������� �� ��������� ����.
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
                "��� - ��� ����������.", 5f);

        placeMeepleBtn.interactable = false;
        endTurnBtn.interactable = false;
        meepleSpawner.StartPlacingMeeple();
    }

    /// <summary>
    /// ������� �������� �End Turn�.
    /// </summary>
    private void OnEndTurn()
    {
        if (!tilePlaced) return;

        audioManager.PlaySFX(audioManager.buttonClick);

        // ϳ�������� ���� �� ����� �������� ���������
        StructureScorer.ScoreCompletedStructures(board, playerManager, uiManager);

        // ���������� ����� ���
        if (deckManager.IsEmpty())
        {
            EndGame();
            return;
        }

        // ��������� ��� ���������� ������
        playerManager.NextPlayer();
        NextTurnSetup();
    }

    /// <summary>
    /// ����������� MeepleSpawner, ���� ��� ������ �����������.
    /// </summary>
    public void OnMeeplePlaced()
    {
        audioManager.PlaySFX(audioManager.tilePlaced);

        meeplePlaced = true;
        endTurnBtn.interactable = true;

        var current = playerManager.GetCurrentPlayer();
        current.UseMeeple();
        uiManager.UpdatePlayerMeeples(playerManager.CurrentPlayerIndex, current.MeepleCount);

        // ��������� ����������� �� ����-�� �����
        board.ClearAllMeepleSlotsExcept(null);
    }

    /// <summary>
    /// ����� ���������� ���.
    /// </summary>
    private void EndGame()
    {
        // ������ ���� �� �� �������� ���������
        StructureScorer.ScoreEndGameStructures(board, playerManager, uiManager);

        // ������� �� ������
        chooseTileBtn.interactable = false;
        placeMeepleBtn.interactable = false;
        endTurnBtn.interactable = false;

        // �������� ����� ���� ���������
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
