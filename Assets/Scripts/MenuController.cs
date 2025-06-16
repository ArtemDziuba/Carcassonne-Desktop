using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

// ����, �� ������� �� ������ ���� � ��������� ���� �� �� ���������� �� �����
public class MenuController : MonoBehaviour
{
    public GameObject helpScreen;
    public GameObject pauseTint; // ������ �� �����������
    public GameObject exitScreen;
    public GameObject settingsScreen;
    public GameObject infoScreen;

    public CameraControl cameraControl;
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
    }

    public void OnNewGameClicked()
    {
        //audioManager.PlaySFX(audioManager.buttonClick);
        SceneManager.LoadScene("PlayerSetupScene");
    }     

    public void OnSavesClicked()
    {
        audioManager.PlaySFX(audioManager.buttonClick);
        SceneManager.LoadScene("SavesScene");
    }

    public void OnSettingsClicked()
    {
        audioManager.PlaySFX(audioManager.buttonClick);

        pauseTint.SetActive(true);
        pauseTint.transform.SetAsLastSibling(); // ���������� �� ������ ���
        settingsScreen.SetActive(true);
    }

    public void OnConfirmSettingsClicked()
    {
        cameraControl.SetZoomSpeed();
        cameraControl.SetPanSpeed();
        audioManager.SetVolume();

        audioManager.PlaySFX(audioManager.buttonClick);

        pauseTint.SetActive(false);
        settingsScreen.SetActive(false);
    }

    public void OnAdditionalInfoClicked()
    {
        audioManager.PlaySFX(audioManager.buttonClick);

        pauseTint.SetActive(true);
        pauseTint.transform.SetAsLastSibling(); // ���������� �� ������ ���
        infoScreen.SetActive(true);
    }

    public void OnHelpClicked()
    {
        audioManager.PlaySFX(audioManager.buttonClick);
        if (helpScreen != null)
        {
            helpScreen.SetActive(true);
            helpScreen.transform.SetAsLastSibling(); // ���������� �� ������ ���
        }
    }

    public void OnExitClicked()
    {
        audioManager.PlaySFX(audioManager.buttonClick);
        pauseTint.SetActive(true);
        pauseTint.transform.SetAsLastSibling(); // ���������� �� ������ ���
        exitScreen.SetActive(true);
    }

    public void OnDenyClicked()
    {
        audioManager.PlaySFX(audioManager.buttonClick);
        pauseTint.SetActive(false);
        exitScreen.SetActive(false);
        settingsScreen.SetActive(false);
        infoScreen.SetActive(false);
    }

    public void OnConfirmClicked()
    {
        Application.Quit();
    }
}