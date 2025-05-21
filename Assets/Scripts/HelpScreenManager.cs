using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class HelpScreenManager : MonoBehaviour
{
    [Header("Основна панель")]
    public GameObject helpScreen;

    [Header("Зображення у ScrollView")]
    public GameObject generalImage;
    public GameObject controlsImage;
    public GameObject roadsImage;
    public GameObject citiesImage;
    public GameObject monasteriesImage;
    public GameObject fieldsImage;
    public GameObject endGameImage;

    private List<GameObject> allImages;

    [SerializeField] private ScrollRect scrollRect;

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
    }

    /// <summary>
    /// Приховує всі зображення
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
    }

    public void ShowControls()
    {
        HideAllImages();
        controlsImage.SetActive(true);
        ScrollToTop();
    }

    public void ShowRoads()
    {
        HideAllImages();
        roadsImage.SetActive(true);
        ScrollToTop();
    }

    public void ShowCities()
    {
        HideAllImages();
        citiesImage.SetActive(true);
        ScrollToTop();
    }

    public void ShowMonasteries()
    {
        HideAllImages();
        monasteriesImage.SetActive(true);
        ScrollToTop();
    }

    public void ShowFields()
    {
        HideAllImages();
        fieldsImage.SetActive(true);
        ScrollToTop();
    }

    public void ShowEndGame()
    {
        HideAllImages();
        endGameImage.SetActive(true);
        ScrollToTop();
    }

    public void OnReturnClicked()
    {
        helpScreen.SetActive(false);
    }

    private void ScrollToTop()
    {
        Canvas.ForceUpdateCanvases(); // важливо для правильного макета
        scrollRect.verticalNormalizedPosition = 1f; // 1 = верх, 0 = низ
    }
}
