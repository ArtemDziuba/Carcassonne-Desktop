using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Клас, що відповідає за UI на сцені налаштування гри
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

    public void Initialize(int id, List<Sprite> sprites, int defaultMeepleIndex)
    {
        playerId = id;
        meepleSprites = sprites;
        MeepleDropdownPanel.SetActive(false);
        selectedSpriteIndex = defaultMeepleIndex;

        if (selectedSpriteIndex >= 0 && selectedSpriteIndex < meepleSprites.Count)
            MeepleImage.sprite = meepleSprites[selectedSpriteIndex];

        // Встановлюємо ім'я за замовчуванням 
        if (NameInputField.placeholder != null)
        {
            var placeholder = NameInputField.placeholder.GetComponent<TextMeshProUGUI>();
            if (placeholder != null)
                placeholder.text = $"Гравець {playerId + 1}";
        }

        MeepleSelectBtn.onClick.AddListener(ToggleMeepleDropdown);
    }

    private void ToggleMeepleDropdown()
    {
        bool isActive = MeepleDropdownPanel.activeSelf;
        MeepleDropdownPanel.SetActive(!isActive);

        if (!isActive)
        {
            // Очищаємо попередні кнопки
            foreach (Transform child in MeepleDropdownPanel.transform)
                Destroy(child.gameObject);

            // Створення кнопок для всіх, крім обраного міпла
            for (int i = 0; i < meepleSprites.Count; i++)
            {
                if (i == selectedSpriteIndex)
                    continue; // Пропускаємо вже вибраний міпл

                Sprite sprite = meepleSprites[i];
                int index = i; // копія для лямбди

                GameObject btnObj = Instantiate(MeepleOptionButtonPrefab, MeepleDropdownPanel.transform);
                var img = btnObj.GetComponentInChildren<Image>();
                if (img != null)
                    img.sprite = sprite;

                var btn = btnObj.GetComponent<Button>();
                if (btn != null)
                {
                    btn.onClick.AddListener(() =>
                    {
                        // Запит на зміну міпла з унікальністю
                        PlayerSetupMenu.Instance.RequestMeepleChange(index, this);

                        // Закриваємо меню та очищуємо кнопки
                        foreach (Transform child in MeepleDropdownPanel.transform)
                            Destroy(child.gameObject);
                        MeepleDropdownPanel.SetActive(false);
                    });
                }
            }
        }
    }

    public void SetMeeple(int index)
    {
        if (index >= 0 && index < meepleSprites.Count)
        {
            selectedSpriteIndex = index;
            MeepleImage.sprite = meepleSprites[index];
        }
    }

    public int GetMeepleId()
    {
        return selectedSpriteIndex;
    }

    public Player ToPlayer()
    {
        string name = string.IsNullOrWhiteSpace(NameInputField.text)
            ? $"Гравець {playerId + 1}"
            : NameInputField.text;

        return new Player(playerId, selectedSpriteIndex, name);
    }
}
