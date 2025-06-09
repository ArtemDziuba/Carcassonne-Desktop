using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

// Клас, що відповідає за відображення збережень у меню збережень
public class SaveEntryUI : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI saveNameText;
    public TextMeshProUGUI playerCountText;
    public Image thumbnailImage;
    public Button deleteButton;

    // Інформація про збереження
    private string saveId;
    private Action<string> onLoadCallback;
    private Action<string> onDeleteCallback;

    /// <summary>
    /// Ініціалізує UI елемент збереження.
    /// </summary>
    /// <param name="id">ID збереження (наприклад, ім’я файлу)</param>
    /// <param name="saveName">Назва збереження</param>
    /// <param name="playerCount">Кількість гравців</param>
    /// <param name="thumbnail">Зображення-заглушка (може бути null)</param>
    /// <param name="onLoad">Що робити при натисканні</param>
    /// <param name="onDelete">Що робити при видаленні</param>
    public void Initialize(string id, string saveName, int playerCount, Sprite thumbnail,
        Action<string> onLoad, Action<string> onDelete)
    {
        saveId = id;
        saveNameText.text = saveName;
        playerCountText.text = $"Гравців: {playerCount}";
        thumbnailImage.sprite = thumbnail;

        onLoadCallback = onLoad;
        onDeleteCallback = onDelete;

        GetComponent<Button>().onClick.AddListener(() => onLoadCallback?.Invoke(saveId));
        deleteButton.onClick.AddListener(() => onDeleteCallback?.Invoke(saveId));
    }
}
