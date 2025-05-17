using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerEntryUI : MonoBehaviour
{
    public int playerId;

    public TMP_InputField NameInputField;
    public Image MeepleImage;
    public Button MeepleSelectBtn;
    public GameObject MeepleDropdownPanel;
    public GameObject MeepleOptionButtonPrefab;

    private List<Sprite> meepleSprites;
    private int selectedSpriteIndex = 0;

    public void Initialize(int id, List<Sprite> sprites)
    {
        playerId = id;
        meepleSprites = sprites;
        MeepleDropdownPanel.SetActive(false);

        // Встановлюємо ім'я за замовчуванням
        if (NameInputField.placeholder != null)
        {
            var placeholder = NameInputField.placeholder?.GetComponent<TextMeshProUGUI>();
            if (placeholder != null)
            {
                placeholder.text = $"Гравець {playerId + 1}";
                Debug.Log($"Placeholder оновлено: {placeholder.text}");
            }
        }

        // Встановлюємо міпл за замовчуванням
        if (meepleSprites.Count > 0)
        {
            MeepleImage.sprite = meepleSprites[0];
            selectedSpriteIndex = 0;
        }
        
        MeepleSelectBtn.onClick.AddListener(ToggleMeepleDropdown);
    }

    private void ToggleMeepleDropdown()
    {
        bool isActive = MeepleDropdownPanel.activeSelf;
        MeepleDropdownPanel.SetActive(!isActive);

        if (!isActive && MeepleDropdownPanel.transform.childCount == 0)
        {
            // Створення кнопок
            for (int i = 0; i < meepleSprites.Count; i++)
            {
                Sprite sprite = meepleSprites[i];
                int index = i; // локальна копія

                GameObject btnObj = Instantiate(MeepleOptionButtonPrefab, MeepleDropdownPanel.transform);
                var img = btnObj.GetComponentInChildren<Image>();
                if (img != null) img.sprite = sprite;

                var btn = btnObj.GetComponent<Button>();
                if (btn != null)
                {
                    btn.onClick.AddListener(() =>
                    {
                        MeepleImage.sprite = sprite;
                        selectedSpriteIndex = index;
                        MeepleDropdownPanel.SetActive(false);
                    });
                }
            }
        }
    }

    public Player ToPlayer()
    {
        string name = string.IsNullOrWhiteSpace(NameInputField.text)
            ? $"Гравець {playerId + 1}"
            : NameInputField.text;

        return new Player(playerId, selectedSpriteIndex, name);
    }
}
