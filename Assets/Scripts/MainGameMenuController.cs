using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

// ����, �� ������� �� ���� ������������� � ��
public class MainGameMenuController : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject pauseTint; // ������ �� ����������� + ����
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
    public GameObject settingsScreen;

    public CameraControl cameraControl;
            
    private bool isPaused = false;

    public AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    void Start()
    {
        // �������� PauseTint �� �������
        if (pauseTint != null)
            pauseTint.SetActive(false);

        // ����'���� ������
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
        audioManager.PlaySFX(audioManager.buttonClick);
        isPaused = true;
        Time.timeScale = 0f; // ��������� ���
        pauseTint.SetActive(true);
        pauseTint.transform.SetAsLastSibling(); // ���������� �� ������ ���
    }

    public void OnSettingsClicked()
    {
        audioManager.PlaySFX(audioManager.buttonClick);

        settingsScreen.SetActive(true);
    }

    public void OnConfirmSettingsClicked()
    {
        cameraControl.SetZoomSpeed();
        cameraControl.SetPanSpeed();
        audioManager.SetVolume();

        audioManager.PlaySFX(audioManager.buttonClick);

        settingsScreen.SetActive(false);
    }

    private void OnResumePressed()
    {
        audioManager.PlaySFX(audioManager.buttonClick);
        isPaused = false;
        Time.timeScale = 1f; // ���������� ���
        pauseTint.SetActive(false);
    }

    private void OnExitToMenuPressed()
    {
        audioManager.PlaySFX(audioManager.buttonClick);
        Time.timeScale = 1f; // ³��������� ��� ����� ���������
        SceneManager.LoadScene("MainMenu");
    }

    public void OnHelpPressed()
    {
        if (helpScreen != null)
        {
            audioManager.PlaySFX(audioManager.buttonClick);
            helpScreen.SetActive(true);
            helpScreen.transform.SetAsLastSibling(); // ���������� �� ������ ���
        }
    }

    public void OnSaveGamePressed()
    {
        audioManager.PlaySFX(audioManager.buttonClick);
        string saveId = $"Save_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}";
        SaveSystem.SaveGame(saveId, board, playerManager, deck, turnManager);
        ToastManager.Instance.ShowToast(ToastType.Success, "��� ���������.");
    }
}
