using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

// Клас, що відповідає за роботу меню у головному меню та за повернення до нього
public class MenuController : MonoBehaviour
{
    public GameObject helpScreen;
    public GameObject pauseTint; // панель із затемненням + меню

    void Start()
    {
        // Вимикаємо PauseTint на початку
        if (pauseTint != null)
            pauseTint.SetActive(false);
    }

    public void OnNewGameClicked()
    {
        SceneManager.LoadScene("PlayerSetupScene");
    }

    public void OnReturnToMenuClicked()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void OnSavesClicked()
    {
        SceneManager.LoadScene("SavesScene");
    }

    public void OnSettingsClicked()
    {
        //TODO
    }

    public void OnAdditionalInfoClicked()
    {
        //TODO
    }

    public void OnHelpClicked()
    {
        if (helpScreen != null)
        {
            helpScreen.SetActive(true);
            helpScreen.transform.SetAsLastSibling(); // перемістити на верхній шар
        }
    }

    public void OnExitClicked()
    {
        pauseTint.SetActive(true);
        pauseTint.transform.SetAsLastSibling(); // перемістити на верхній шар
    }

    public void OnDenyClicked()
    {
        pauseTint.SetActive(false);
    }

    public void OnConfirmClicked()
    {
        Application.Quit();
    }
}