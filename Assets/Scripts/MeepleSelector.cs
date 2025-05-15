using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MeepleSelector : MonoBehaviour
{
    public Button MeepleSelectBtn;
    public GameObject MeepleDropdownPanel;
    public GameObject MeepleButtonPrefab;
    public Image MeepleImageTarget;

    public List<Sprite> meepleSprites;

    private void Start()
    {
        MeepleSelectBtn.onClick.AddListener(ShowMeepleDropdown);
        MeepleDropdownPanel.SetActive(false);
    }

    void ShowMeepleDropdown()
    {
        Debug.Log("Викликано ShowMeepleDropdown()");
        MeepleDropdownPanel.SetActive(!MeepleDropdownPanel.activeSelf);

        if (MeepleDropdownPanel.transform.childCount == 0)
        {
            foreach (Sprite sprite in meepleSprites)
            {
                Debug.Log($"Створюю кнопку для міпла: {sprite.name}");
                GameObject newBtn = Instantiate(MeepleButtonPrefab, MeepleDropdownPanel.transform);
                newBtn.GetComponentInChildren<Image>().sprite = sprite;
                newBtn.GetComponent<Button>().onClick.AddListener(() => OnMeepleSelected(sprite));
                Debug.Log($"Створено міпл: {sprite.name}");
            }
        }
    }

    void OnMeepleSelected(Sprite selectedMeeple)
    {
        MeepleImageTarget.sprite = selectedMeeple;
        MeepleDropdownPanel.SetActive(false);
    }
}
