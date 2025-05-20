using UnityEngine;
using UnityEngine.SceneManagement;

public class HelpSceneManager : MonoBehaviour
{
    private static string previousSceneName;

    public static void OpenHelpScene()
    {
        // �������� ������� ����� ����� ���������
        previousSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene("HelpScene", LoadSceneMode.Additive);
    }

    public void OnReturnClicked()
    {
        // ��������� HelpScene
        SceneManager.UnloadSceneAsync("HelpScene");

        // ���� ��������� ����� ����� ���� � ����������� ��
        if (!string.IsNullOrEmpty(previousSceneName) && Application.CanStreamedLevelBeLoaded(previousSceneName))
        {
            SceneManager.LoadScene(previousSceneName);
        }
        else
        {
            ToastBridge.ShowInNextScene(ToastType.Error,
                "���� ���� �� ���, ����������� ������� ����", "MainMenu");
            SceneManager.LoadScene("MainMenu");
        }
    }
}
