using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainGameMenuController : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject pauseTint; // ������ �� ����������� + ����
    public Button pauseBtn;
    public Button resumeGameBtn;
    public Button exitToMenuBtn;

    private bool isPaused = false;

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
    }

    private void OnPausePressed()
    {
        isPaused = true;
        Time.timeScale = 0f; // ��������� ���
        pauseTint.SetActive(true);
        pauseTint.transform.SetAsLastSibling(); // ���������� �� ������ ���
    }

    private void OnResumePressed()
    {
        isPaused = false;
        Time.timeScale = 1f; // ���������� ���
        pauseTint.SetActive(false);
    }

    private void OnExitToMenuPressed()
    {
        Time.timeScale = 1f; // ³��������� ��� ����� ���������
        SceneManager.LoadScene("MainMenu");
    }
}
