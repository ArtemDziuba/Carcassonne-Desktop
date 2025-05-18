using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

public class MenuController : MonoBehaviour
{
    

    public void OnNewGameClicked()
    {
        SceneManager.LoadScene("PlayerSetupScene");
    }

    public void OnReturnToMenuClicked()
    {
        SceneManager.LoadScene("MainMenu");
    }
    
}