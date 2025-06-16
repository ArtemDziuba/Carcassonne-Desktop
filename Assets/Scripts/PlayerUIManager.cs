using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

// Клас, що відповідає за керування UI гравців безпосередньо у грі
public class PlayerUIManager : MonoBehaviour
{
    [Header("Live Player UI")]
    public GameObject playerUIPrefab;
    public RectTransform canvasRect;
    public Sprite[] meepleSprites;
    private readonly List<PlayerUIItem> playerUIItems = new();

    [Header("Layout Settings")]
    public Vector2 startOffset = new Vector2(80, -80);
    public float elementWidth = 260;
    public float spacing = 30;

    [Header("Game Over UI")]
    [Tooltip("Панель кінця гри (має бути неактивною в інспекторі)")]
    public GameObject gameOverPanel;
    [Tooltip("RectTransform 'Content' під Scroll View")]
    public RectTransform gameOverContent;
    [Tooltip("Prefab одного рядка результатів")]
    public GameObject gameOverEntryPrefab;

    [Header("References")]
    public PlayerManager playerManager;
    AudioManager audioManager;

    void Awake()
    {
        // Підпишемо кнопку закриття, якщо вона є
        if (gameOverPanel != null)
        {
            var closeBtn = gameOverPanel.transform
                .Find("CloseButton")
                ?.GetComponent<Button>();
            if (closeBtn != null)
                closeBtn.onClick.AddListener(HideGameOverUI);
            else
                Debug.LogWarning("CloseButton не знайдено у GameOverPanel");
        }

        // Забезпечимо: ScrollRect — тільки вертикальний
        if (gameOverContent != null)
        {
            var scrollRect = gameOverContent
                .GetComponentInParent<ScrollRect>();
            if (scrollRect != null)
            {
                scrollRect.horizontal = false;
                scrollRect.vertical = true;
            }
        }

        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    public void InitializeUI(System.Collections.Generic.List<Player> players)
    {
        // Live UI
        foreach (var item in playerUIItems)
            Destroy(item.gameObject);
        playerUIItems.Clear();

        // Ховаємо GameOverPanel на старті
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        for (int i = 0; i < players.Count; i++)
        {
            var player = players[i];
            var uiObj = Instantiate(playerUIPrefab, canvasRect);
            var rect = uiObj.GetComponent<RectTransform>();

            float x = startOffset.x + i * (elementWidth + spacing);
            float y = startOffset.y;
            rect.anchoredPosition = new Vector2(x, y);

            var ui = uiObj.GetComponent<PlayerUIItem>();
            ui.UpdateUI(
                meepleSprites[player.MeepleSpriteIndex],
                player.MeepleCount,
                player.Score
            );

            if (player.MeepleSpriteIndex == 1)
                ui.meepleCountText.color = Color.black;

            playerUIItems.Add(ui);
        }
    }

    public void SetActivePlayer(int index)
    {
        for (int i = 0; i < playerUIItems.Count; i++)
            playerUIItems[i].SetActive(i == index);
    }

    public void UpdatePlayerMeeples(int index, int count)
        => playerUIItems[index].meepleCountText.text = count.ToString();

    public void UpdatePlayerScore(int index, int score)
        => playerUIItems[index].scoreText.text = score.ToString();

    public void ShowGameOverUI()
    {
        if (gameOverPanel == null || gameOverEntryPrefab == null || gameOverContent == null)
        {
            Debug.LogError("PlayerUIManager: перевірте, що gameOverPanel, gameOverContent та gameOverEntryPrefab прив'язані!");
            return;
        }

        // 1) Показуємо поверх усього UI
        gameOverPanel.SetActive(true);
        gameOverPanel.transform.SetAsLastSibling();

        // 2) Очищаємо старі записи
        foreach (Transform child in gameOverContent)
            Destroy(child.gameObject);

        // 3) Сортуємо гравців за рахунком
        var sorted = playerManager.Players
            .OrderByDescending(p => p.Score)
            .ToList();

        // 4) Створюємо записи
        for (int i = 0; i < sorted.Count; i++)
        {
            var player = sorted[i];
            var entry = Instantiate(gameOverEntryPrefab, gameOverContent).transform;

            entry.Find("RankText")
                 .GetComponent<TextMeshProUGUI>()
                 .text = (i + 1).ToString();

            var gameOverEntryP1 = entry.Find("GameOverEntryP1").GetComponent<Image>();
            gameOverEntryP1.gameObject.SetActive(false);
            var gameOverEntryP2 = entry.Find("GameOverEntryP2").GetComponent<Image>();
            gameOverEntryP2.gameObject.SetActive(false);
            var gameOverEntryP3 = entry.Find("GameOverEntryP3").GetComponent<Image>();
            gameOverEntryP3.gameObject.SetActive(false);

            var gameOverNumP1 = entry.Find("GameOverNumP1").GetComponent<Image>();
            gameOverNumP1.gameObject.SetActive(false);
            var gameOverNumP2 = entry.Find("GameOverNumP2").GetComponent<Image>();
            gameOverNumP2.gameObject.SetActive(false);
            var gameOverNumP3 = entry.Find("GameOverNumP3").GetComponent<Image>();
            gameOverNumP3.gameObject.SetActive(false);

            if (i == 0)
            {
                gameOverEntryP1.gameObject.SetActive(true);
                gameOverNumP1.gameObject.SetActive(true);
            }
            else if (i == 1)
            {
                gameOverEntryP2.gameObject.SetActive(true);
                gameOverNumP2.gameObject.SetActive(true);
            }
            else if (i == 2)
            {
                gameOverEntryP3.gameObject.SetActive(true);
                gameOverNumP3.gameObject.SetActive(true);
            }
           
            entry.Find("MeepleImage")
                 .GetComponent<Image>()
                 .sprite = meepleSprites[player.MeepleSpriteIndex];

            entry.Find("NameText")
                 .GetComponent<TextMeshProUGUI>()
                 .text = player.Name;

            entry.Find("ScoreText")
                 .GetComponent<TextMeshProUGUI>()
                 .text = player.Score.ToString();
            // StarImage спрайт уже налаштований у префабі
        }
    }

    public void HideGameOverUI()
    {
        audioManager.PlaySFX(audioManager.tilePlaced);
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }
}
