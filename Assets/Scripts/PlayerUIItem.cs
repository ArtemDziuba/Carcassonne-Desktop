using TMPro;
using UnityEngine;
using UnityEngine.UI;

// ����, �� ������� �� UI ������� ������������� � ��
public class PlayerUIItem : MonoBehaviour
{
    [Header("Background & State")]
    public Image background;
    public Sprite backgroundNormal;
    public Sprite backgroundActive;

    [Header("Content")]
    public Image meepleImage;
    public TextMeshProUGUI meepleCountText;
    public TextMeshProUGUI scoreText;

    /// <summary>
    /// ������ ��� �������� ����� ������ (�����������).
    /// </summary>
    /// <param name="active">�� �������� ����� �������</param>
    public void SetActive(bool active)
    {
        if (background != null && backgroundActive != null && backgroundNormal != null)
        {
            background.sprite = active ? backgroundActive : backgroundNormal;
        }
    }

    /// <summary>
    /// ������� UI �������� ������.
    /// </summary>
    /// <param name="meepleSprite">������ ����</param>
    /// <param name="meepleCount">ʳ������ ����</param>
    /// <param name="score">����</param>
    public void UpdateUI(Sprite meepleSprite, int meepleCount, int score)
    {
        if (meepleImage != null)
            meepleImage.sprite = meepleSprite;

        if (meepleCountText != null)
            meepleCountText.text = meepleCount.ToString();

        if (scoreText != null)
            scoreText.text = score.ToString();
    }
}
