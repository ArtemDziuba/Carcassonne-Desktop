using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

public class MenuController : MonoBehaviour
{
    public Button StartGameBtn;

    private void Start()
    {
        StartGameBtn.onClick.AddListener(OnStartGameClicked);
    }

    public void OnNewGameClicked()
    {
        SceneManager.LoadScene("PlayerSetupScene");
    }

    public void OnReturnToMenuClicked()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void OnStartGameClicked()
    {
        List<Player> players = PlayerSetupMenu.Instance.GetPlayers();
        GameConfig.Instance.Players = players;

        // Завантажити основну ігрову сцену
        SceneManager.LoadScene("MainGame");
    }
}