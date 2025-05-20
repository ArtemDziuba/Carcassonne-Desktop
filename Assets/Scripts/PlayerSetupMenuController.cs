using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerSetupMenu : MonoBehaviour
{
    public static PlayerSetupMenu Instance; // ��� ����������� �������

    public Button AddPlayerBtn;
    public Button RemovePlayerBtn;
    public Button StartGameBtn;
    public Transform PlayerContainer;
    public GameObject PlayerEntryPrefab;
    public List<Sprite> MeepleSprites;

    private List<PlayerEntryUI> playerEntries = new(); // ������ UI �������
    private const int MaxPlayers = 5;
    private const int MinPlayers = 2;
    private int nextPlayerId = 0;
    private Stack<int> availableIds = new(); // ������� ID

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void Start()
    {
        AddPlayerBtn.onClick.AddListener(CreateNewPlayerEntry);
        RemovePlayerBtn.onClick.AddListener(RemoveLastPlayerEntry);
        StartGameBtn.onClick.AddListener(OnStartGameClicked);

        // ��������� 2 ��������� ������
        for (int i = 0; i < MinPlayers; i++)
            CreateNewPlayerEntry();
    }

    private void CreateNewPlayerEntry()
    {
        if (playerEntries.Count >= MaxPlayers)
        {
            ToastManager.Instance.ShowToast(ToastType.Warning, "����������� ������� ������� ���������.");
            Debug.LogWarning("����������� ������� ������� ���������.");
            return;
        }

        GameObject entryGO = Instantiate(PlayerEntryPrefab, PlayerContainer);
        PlayerEntryUI entryUI = entryGO.GetComponent<PlayerEntryUI>();

        int freeMeepleIndex = FindFirstAvailableMeepleIndex();
        int playerId = availableIds.Count > 0 ? availableIds.Pop() : nextPlayerId++;
        entryUI.Initialize(playerId, MeepleSprites, freeMeepleIndex);

        playerEntries.Add(entryUI);
    }

    private void RemoveLastPlayerEntry()
    {
        if (playerEntries.Count <= MinPlayers)
        {
            ToastManager.Instance.ShowToast(ToastType.Warning, "̳������� ������� ������� � 2.");
            Debug.LogWarning("̳������� ������� ������� � 2. ��������� ����������.");
            return;
        }

        PlayerEntryUI lastEntry = playerEntries[playerEntries.Count - 1];
        availableIds.Push(lastEntry.playerId);
        playerEntries.RemoveAt(playerEntries.Count - 1);
        Destroy(lastEntry.gameObject);
    }

    public List<Player> GetPlayers()
    {
        List<Player> result = new();
        foreach (var entry in playerEntries)
        {
            result.Add(entry.ToPlayer());
        }
        return result;
    }

    /// <summary>
    /// ������� ������ �� ���� ����. ���� ����� ����� ��� �� ����� ��� � ���������� ����.
    /// </summary>
    public void RequestMeepleChange(int newMeepleId, PlayerEntryUI requester)
    {
        foreach (var other in playerEntries)
        {
            if (other == requester) continue;

            if (other.GetMeepleId() == newMeepleId)
            {
                int oldRequesterMeeple = requester.GetMeepleId();

                // ̳����� ������ ����
                requester.SetMeeple(newMeepleId);
                other.SetMeeple(oldRequesterMeeple);

                Debug.Log($"���� ���� �� ������� {requester.playerId} �� {other.playerId}");
                return;
            }
        }

        // ���� ��� �� �������� � ������ ������������
        requester.SetMeeple(newMeepleId);
    }

    private int FindFirstAvailableMeepleIndex()
    {
        HashSet<int> usedIndices = new();

        foreach (var entry in playerEntries)
        {
            usedIndices.Add(entry.GetMeepleId());
        }

        for (int i = 0; i < MeepleSprites.Count; i++)
        {
            if (!usedIndices.Contains(i))
                return i;
        }

        return 0; // fallback, ���� �� ������ (�� �� ������� ��� 5 ������� � 5 �����)
    }

    private void OnStartGameClicked()
    {
        List<Player> players = Instance.GetPlayers();
        GameConfig.Instance.Players = players;

        // ����������� ������� ������ �����
        SceneManager.LoadScene("MainGame");
    }
}
