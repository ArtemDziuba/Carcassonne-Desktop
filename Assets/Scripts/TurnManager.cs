using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TurnManager : MonoBehaviour
{
    public PlayerManager playerManager;
    public TileSpawner tileSpawner;
    public MeepleSpawner meepleSpawner;
    public Board board;

    public Button chooseTileBtn;
    public Button placeMeepleBtn;
    public Button endTurnBtn;

    public TextMeshProUGUI chooseTileBtnText;
    public TileDeckManager deckManager;
    public PlayerUIManager uiManager;

    private bool tilePlaced = false;
    private bool meeplePlaced = false;

    void Start()
    {
        playerManager.CreatePlayers(playerManager.playerCount);

        uiManager.InitializeUI(playerManager.Players);
        uiManager.SetActivePlayer(playerManager.CurrentPlayerIndex);

        chooseTileBtn.onClick.AddListener(OnChooseTile);
        placeMeepleBtn.onClick.AddListener(OnPlaceMeeple);
        endTurnBtn.onClick.AddListener(OnEndTurn);

        StartTurn();
    }

    void StartTurn()
    {
        var currentPlayer = playerManager.GetCurrentPlayer();

        Debug.Log($"������� ���� ������ {currentPlayer.PlayerId}");

        tilePlaced = false;
        meeplePlaced = false;

        meepleSpawner.currentPlayerIndex = currentPlayer.MeepleSpriteIndex;

        chooseTileBtn.interactable = true;
        placeMeepleBtn.interactable = false;
        endTurnBtn.interactable = false;

        board.ClearAllMeepleSlotsExcept(null);

        // ��������� ����� ������
        chooseTileBtnText.text = $"{deckManager.fullDeck.Count}";

        // �������� ��������� ������
        uiManager.SetActivePlayer(playerManager.CurrentPlayerIndex);
    }

    void OnChooseTile()
    {
        tileSpawner.SpawnNextTile();

        chooseTileBtn.interactable = false;

        var currentPlayer = playerManager.GetCurrentPlayer();

        // ���� � ���� � ������ �������, ������ � ��������� + �������� �����
        if (currentPlayer.HasMeeples())
        {
            placeMeepleBtn.interactable = true;
        }
        else
        {
            placeMeepleBtn.interactable = false;
            board.ClearAllMeepleSlotsExcept(null);
            Debug.Log($"[TurnManager] � ������ {currentPlayer.PlayerId} ���� ����.");
        }

        endTurnBtn.interactable = true;

        tilePlaced = true;

        chooseTileBtnText.text = $"{deckManager.fullDeck.Count}";
    }

    void OnPlaceMeeple()
    {
        var currentPlayer = playerManager.GetCurrentPlayer();

        if (!currentPlayer.HasMeeples())
        {
            Debug.Log($"[TurnManager] � ������ {currentPlayer.PlayerId} ���� ����.");
            board.ClearAllMeepleSlotsExcept(null);
            return;
        }

        meepleSpawner.StartPlacingMeeple();
        placeMeepleBtn.interactable = false;
    }

    void OnEndTurn()
    {
        if (!tilePlaced) return;

        playerManager.NextPlayer();
        StartTurn();
    }

    public void OnMeeplePlaced()
    {
        var currentPlayer = playerManager.GetCurrentPlayer();
        currentPlayer.UseMeeple();
        meeplePlaced = true;

        board.ClearAllMeepleSlotsExcept(null);
        placeMeepleBtn.interactable = false;

        // ��������� UI
        uiManager.UpdatePlayerMeeples(playerManager.CurrentPlayerIndex, currentPlayer.MeepleCount);
    }
}
