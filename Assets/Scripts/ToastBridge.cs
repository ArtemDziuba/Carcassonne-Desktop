using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ToastBridge : MonoBehaviour
{
    private static ToastType toastType;
    private static string toastMessage;
    private static bool shouldShow = false;
    private static string sceneToWatch;

    public static void ShowInNextScene(ToastType type, string message, string expectedScene = "MainMenu")
    {
        toastType = type;
        toastMessage = message;
        shouldShow = true;
        sceneToWatch = expectedScene;

        GameObject bridge = new GameObject("ToastBridge");
        bridge.AddComponent<ToastBridge>();
        DontDestroyOnLoad(bridge);
    }

    private void Start()
    {
        StartCoroutine(WaitUntilSceneLoadedAndShowToast());
    }

    private IEnumerator WaitUntilSceneLoadedAndShowToast()
    {
        // Чекаємо, поки потрібна сцена стане активною
        while (SceneManager.GetActiveScene().name != sceneToWatch)
        {
            yield return null;
        }

        // Чекаємо ToastManager
        while (ToastManager.Instance == null)
        {
            yield return null;
        }

        if (shouldShow)
        {
            ToastManager.Instance.ShowToast(toastType, toastMessage);
            shouldShow = false;
        }

        Destroy(gameObject);
    }
}
