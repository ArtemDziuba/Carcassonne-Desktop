using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainGameMenuController : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject pauseTint; // панель із затемненням + меню
    public Button pauseBtn;
    public Button resumeGameBtn;
    public Button exitToMenuBtn;

    private bool isPaused = false;

    void Start()
    {
        // Вимикаємо PauseTint на початку
        if (pauseTint != null)
            pauseTint.SetActive(false);

        // Прив'язки кнопок
        if (pauseBtn != null)
            pauseBtn.onClick.AddListener(OnPausePressed);

        if (resumeGameBtn != null)
            resumeGameBtn.onClick.AddListener(OnResumePressed);

        if (exitToMenuBtn != null)
            exitToMenuBtn.onClick.AddListener(OnExitToMenuPressed);
    }

    private void OnPausePressed()
    {
        isPaused = true;
        Time.timeScale = 0f; // Зупиняємо гру
        pauseTint.SetActive(true);
        pauseTint.transform.SetAsLastSibling(); // перемістити на верхній шар
    }

    private void OnResumePressed()
    {
        isPaused = false;
        Time.timeScale = 1f; // Продовжуємо гру
        pauseTint.SetActive(false);
    }

    private void OnExitToMenuPressed()
    {
        Time.timeScale = 1f; // Відновлюємо гру перед переходом
        SceneManager.LoadScene("MainMenu");
    }
}
