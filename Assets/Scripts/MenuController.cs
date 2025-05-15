using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void OnNewGameClicked()
    {
        SceneManager.LoadScene("PlayerSetupScene"); // назва сцени має збігатись
    }

    public void OnReturnToMenuClicked()
    {
        Debug.Log("Кнопка натиснута.");
        SceneManager.LoadScene("MainMenu"); // Назва твоєї головної сцени
    }
}