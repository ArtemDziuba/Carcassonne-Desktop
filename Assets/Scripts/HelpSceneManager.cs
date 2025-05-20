using UnityEngine;
using UnityEngine.SceneManagement;

public class HelpSceneManager : MonoBehaviour
{
    private static string previousSceneName;

    public static void OpenHelpScene()
    {
        // Зберігаємо активну сцену перед переходом
        previousSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene("HelpScene", LoadSceneMode.Additive);
    }

    public void OnReturnClicked()
    {
        // Закриваємо HelpScene
        SceneManager.UnloadSceneAsync("HelpScene");

        // Якщо збережена назва сцени існує — завантажуємо її
        if (!string.IsNullOrEmpty(previousSceneName) && Application.CanStreamedLevelBeLoaded(previousSceneName))
        {
            SceneManager.LoadScene(previousSceneName);
        }
        else
        {
            ToastBridge.ShowInNextScene(ToastType.Error,
                "Щось пішло не так, завантажено головне меню", "MainMenu");
            SceneManager.LoadScene("MainMenu");
        }
    }
}
