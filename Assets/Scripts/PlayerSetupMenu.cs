using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSetupMenu : MonoBehaviour
{
    public Button AddPlayerBtn;
    public Button RemovePlayerBtn;
    public Transform PlayerContainer;
    public GameObject PlayerEntryPrefab;
    public List<Sprite> MeepleSprites;

    private List<GameObject> playerEntries = new();
    private const int MaxPlayers = 5;
    private int nextPlayerId = 0;

    private void Start()
    {
        AddPlayerBtn.onClick.AddListener(CreateNewPlayerEntry);
        RemovePlayerBtn.onClick.AddListener(RemoveLastPlayerEntry);
    }

    private void CreateNewPlayerEntry()
    {
        if (playerEntries.Count >= MaxPlayers)
        {
            Debug.LogWarning("Максимальна кількість гравців досягнута.");
            return;
        }

        GameObject entryGO = Instantiate(PlayerEntryPrefab, PlayerContainer);
        PlayerEntryUI entryUI = entryGO.GetComponent<PlayerEntryUI>();
        entryUI.Initialize(playerEntries.Count, MeepleSprites);

        playerEntries.Add(entryGO);
        nextPlayerId++;
    }

    private void RemoveLastPlayerEntry()
    {
        if (playerEntries.Count == 0)
        {
            Debug.LogWarning("Немає гравців для видалення.");
            return;
        }

        GameObject lastEntry = playerEntries[playerEntries.Count - 1];
        playerEntries.RemoveAt(playerEntries.Count - 1);
        Destroy(lastEntry);
    }

    public List<Player> GetPlayers()
    {
        List<Player> result = new();
        for (int i = 0; i < playerEntries.Count; i++)
        {
            var entry = playerEntries[i].GetComponent<PlayerEntryUI>();
            result.Add(entry.ToPlayer());
        }
        return result;
    }
}
