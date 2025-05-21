using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

public class MenuController : MonoBehaviour
{
    public GameObject helpScreen;

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
        // TODO зробити вихід з гри
    }

}