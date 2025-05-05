using UnityEngine;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour
{
    public PlayerManager playerManager;
    public TileSpawner tileSpawner;
    public MeepleSpawner meepleSpawner;

    public Button chooseTileBtn;
    public Button placeMeepleBtn;
    public Button endTurnBtn;

    private bool tilePlaced = false;
    private bool meeplePlaced = false;

    void Start()
    {
        chooseTileBtn.onClick.AddListener(OnChooseTile);
        placeMeepleBtn.onClick.AddListener(OnPlaceMeeple);
        endTurnBtn.onClick.AddListener(OnEndTurn);

        StartTurn();
    }

    void StartTurn()
    {
        Debug.Log($"Початок ходу гравця {playerManager.GetCurrentPlayer().PlayerId}");

        tilePlaced = false;
        meeplePlaced = false;

        meepleSpawner.currentPlayerIndex = playerManager.GetCurrentPlayer().MeepleSpriteIndex;

        chooseTileBtn.interactable = true;
        placeMeepleBtn.interactable = false;
        endTurnBtn.interactable = false;
    }

    void OnChooseTile()
    {
        tileSpawner.SpawnNextTile();

        chooseTileBtn.interactable = false;
        placeMeepleBtn.interactable = true;
        endTurnBtn.interactable = true;

        tilePlaced = true;
    }

    void OnPlaceMeeple()
    {
        meepleSpawner.StartPlacingMeeple();
        placeMeepleBtn.interactable = false;
        meeplePlaced = true;
    }

    void OnEndTurn()
    {
        if (!tilePlaced) return; // не дозволяти завершити хід до розміщення тайлу

        playerManager.NextPlayer();
        StartTurn();
    }
}
