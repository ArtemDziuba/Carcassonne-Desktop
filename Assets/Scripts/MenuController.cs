using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void OnNewGameClicked()
    {
        SceneManager.LoadScene("PlayerSetupScene"); // ����� ����� �� ��������
    }

    public void OnReturnToMenuClicked()
    {
        Debug.Log("������ ���������.");
        SceneManager.LoadScene("MainMenu"); // ����� �� ������� �����
    }
}