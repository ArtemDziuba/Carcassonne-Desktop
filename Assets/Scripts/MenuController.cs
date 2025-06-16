using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

// Клас, що відповідає за роботу меню у головному меню та за повернення до нього
public class MenuController : MonoBehaviour
{
    public GameObject helpScreen;
    public GameObject pauseTint; // панель із затемненням
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
        // Вимикаємо PauseTint на початку
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
        pauseTint.transform.SetAsLastSibling(); // перемістити на верхній шар
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
        pauseTint.transform.SetAsLastSibling(); // перемістити на верхній шар
        infoScreen.SetActive(true);
    }

    public void OnHelpClicked()
    {
        audioManager.PlaySFX(audioManager.buttonClick);
        if (helpScreen != null)
        {
            helpScreen.SetActive(true);
            helpScreen.transform.SetAsLastSibling(); // перемістити на верхній шар
        }
    }

    public void OnExitClicked()
    {
        audioManager.PlaySFX(audioManager.buttonClick);
        pauseTint.SetActive(true);
        pauseTint.transform.SetAsLastSibling(); // перемістити на верхній шар
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