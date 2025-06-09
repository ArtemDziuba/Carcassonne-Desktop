using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

// Клас, що відповідає за меню безпосередньо у грі
public class MainGameMenuController : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject pauseTint; // панель із затемненням + меню
    public Button pauseBtn;
    public Button resumeGameBtn;
    public Button exitToMenuBtn;
    public Button helpBtn;
    public GameObject helpScreen;
    public GameLoader gameLoader; 
    public Board board;
    public TileDeckManager deck;
    public PlayerManager playerManager;
    public TurnManager turnManager;
        
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

        if (helpBtn != null)
            helpBtn.onClick.AddListener(OnHelpPressed);
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

    public void OnHelpPressed()
    {
        if (helpScreen != null) 
        { 
            helpScreen.SetActive(true);
            helpScreen.transform.SetAsLastSibling(); // перемістити на верхній шар
        }
    }

    public void OnSaveGamePressed()
    {
        string saveId = $"Save_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}";
        SaveSystem.SaveGame(saveId, board, playerManager, deck, turnManager);
        ToastManager.Instance.ShowToast(ToastType.Success, "Гру збережено.");
    }
}
