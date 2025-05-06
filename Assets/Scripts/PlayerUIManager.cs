using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIManager : MonoBehaviour
{
    public GameObject playerUIPrefab;
    public RectTransform canvasRect;
    public Sprite[] meepleSprites;
    public List<PlayerUIItem> playerUIItems = new();

    // Початкова позиція
    public Vector2 startOffset = new Vector2(80, -80); // x: справа, y: вниз (Canvas - top-left anchor)
    public float elementWidth = 260;
    public float spacing = 30;

    public void InitializeUI(List<Player> players)
    {
        // Очистити старі
        foreach (var item in playerUIItems)
            Destroy(item.gameObject);
        playerUIItems.Clear();

        for (int i = 0; i < players.Count; i++)
        {
            Player player = players[i];

            GameObject uiObj = Instantiate(playerUIPrefab, canvasRect);
            RectTransform rect = uiObj.GetComponent<RectTransform>();

            // Розрахунок позиції (враховує spacing)
            float x = startOffset.x + i * (elementWidth + spacing);
            float y = startOffset.y;

            rect.anchoredPosition = new Vector2(x, y);

            PlayerUIItem ui = uiObj.GetComponent<PlayerUIItem>();
            ui.UpdateUI(
                meepleSprites[player.MeepleSpriteIndex],
                player.MeepleCount,
                0 // очки — 0 на старті
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
    {
        playerUIItems[index].meepleCountText.text = count.ToString();
    }

    public void UpdatePlayerScore(int index, int score)
    {
        playerUIItems[index].scoreText.text = score.ToString();
    }
}
