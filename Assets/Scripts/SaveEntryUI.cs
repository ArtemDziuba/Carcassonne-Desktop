using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

// ����, �� ������� �� ����������� ��������� � ���� ���������
public class SaveEntryUI : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI saveNameText;
    public TextMeshProUGUI playerCountText;
    public Image thumbnailImage;
    public Button deleteButton;

    // ���������� ��� ����������
    private string saveId;
    private Action<string> onLoadCallback;
    private Action<string> onDeleteCallback;

    /// <summary>
    /// �������� UI ������� ����������.
    /// </summary>
    /// <param name="id">ID ���������� (���������, ��� �����)</param>
    /// <param name="saveName">����� ����������</param>
    /// <param name="playerCount">ʳ������ �������</param>
    /// <param name="thumbnail">����������-�������� (���� ���� null)</param>
    /// <param name="onLoad">�� ������ ��� ���������</param>
    /// <param name="onDelete">�� ������ ��� ��������</param>
    public void Initialize(string id, string saveName, int playerCount, Sprite thumbnail,
        Action<string> onLoad, Action<string> onDelete)
    {
        saveId = id;
        saveNameText.text = saveName;
        playerCountText.text = $"�������: {playerCount}";
        thumbnailImage.sprite = thumbnail;

        onLoadCallback = onLoad;
        onDeleteCallback = onDelete;

        GetComponent<Button>().onClick.AddListener(() => onLoadCallback?.Invoke(saveId));
        deleteButton.onClick.AddListener(() => onDeleteCallback?.Invoke(saveId));
    }
}
