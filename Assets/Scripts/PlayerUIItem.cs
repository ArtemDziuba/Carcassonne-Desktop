using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Клас, що відповідає за UI гравців безпосередньо у грі
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
    /// Активує або деактивує рамку гравця (підсвічування).
    /// </summary>
    /// <param name="active">Чи активний зараз гравець</param>
    public void SetActive(bool active)
    {
        if (background != null && backgroundActive != null && backgroundNormal != null)
        {
            background.sprite = active ? backgroundActive : backgroundNormal;
        }
    }

    /// <summary>
    /// Оновлює UI елементи гравця.
    /// </summary>
    /// <param name="meepleSprite">Спрайт міпла</param>
    /// <param name="meepleCount">Кількість міплів</param>
    /// <param name="score">Очки</param>
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
