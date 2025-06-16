using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

// ����, �� ������� �� ����� ��������
public class HelpScreenManager : MonoBehaviour
{
    [Header("������� ������")]
    public GameObject helpScreen;

    [Header("���������� � ScrollView")]
    public GameObject generalImage;
    public GameObject controlsImage;
    public GameObject roadsImage;
    public GameObject citiesImage;
    public GameObject monasteriesImage;
    public GameObject fieldsImage;
    public GameObject endGameImage;

    private List<GameObject> allImages;

    [SerializeField] private ScrollRect scrollRect;

    AudioManager audioManager;

    void Awake()
    {
        allImages = new List<GameObject>
        {
            generalImage,
            controlsImage,
            roadsImage,
            citiesImage,
            monasteriesImage,
            fieldsImage,
            endGameImage
        };

        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    /// <summary>
    /// ������� �� ����������
    /// </summary>
    private void HideAllImages()
    {
        foreach (var img in allImages)
            img.SetActive(false);
    }

    public void ShowGeneral()
    {
        HideAllImages();
        generalImage.SetActive(true);
        ScrollToTop();
        audioManager.PlaySFX(audioManager.buttonClick);
    }

    public void ShowControls()
    {
        HideAllImages();
        controlsImage.SetActive(true);
        ScrollToTop();
        audioManager.PlaySFX(audioManager.buttonClick);
    }

    public void ShowRoads()
    {
        HideAllImages();
        roadsImage.SetActive(true);
        ScrollToTop();
        audioManager.PlaySFX(audioManager.buttonClick);
    }

    public void ShowCities()
    {
        HideAllImages();
        citiesImage.SetActive(true);
        ScrollToTop();
        audioManager.PlaySFX(audioManager.buttonClick);
    }

    public void ShowMonasteries()
    {
        HideAllImages();
        monasteriesImage.SetActive(true);
        ScrollToTop();
        audioManager.PlaySFX(audioManager.buttonClick);
    }

    public void ShowFields()
    {
        HideAllImages();
        fieldsImage.SetActive(true);
        ScrollToTop();
        audioManager.PlaySFX(audioManager.buttonClick);
    }

    public void ShowEndGame()
    {
        HideAllImages();
        endGameImage.SetActive(true);
        ScrollToTop();
        audioManager.PlaySFX(audioManager.buttonClick);
    }

    public void OnReturnClicked()
    {
        helpScreen.SetActive(false);
        audioManager.PlaySFX(audioManager.buttonClick);
    }

    private void ScrollToTop()
    {
        Canvas.ForceUpdateCanvases(); // ������� ��� ����������� ������
        scrollRect.verticalNormalizedPosition = 1f; // 1 = ����, 0 = ���
    }
}
