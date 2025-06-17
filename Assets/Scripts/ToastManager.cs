using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

// Клас, що відповідає за керування тостами
public class ToastManager : MonoBehaviour
{
    public static ToastManager Instance;

    [Header("Toast Panel (Parent)")]
    public Transform toastPanel;

    [Header("Toast Prefabs")]
    public GameObject toastInfoPrefab;
    public GameObject toastSuccessPrefab;
    public GameObject toastWarningPrefab;
    public GameObject toastErrorPrefab;

    [Header("Display Settings")]
    public float duration = 3f;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void ShowToast(ToastType type, string message, float? customDuration = null)
    {
        GameObject prefab = GetPrefabByType(type);
        if (prefab == null || toastPanel == null) return;

        GameObject toastGO = Instantiate(prefab, toastPanel);
        var text = toastGO.GetComponentInChildren<TextMeshProUGUI>();
        if (text != null) text.text = message;

        var btn = toastGO.GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.AddListener(() => Destroy(toastGO));
        }

        float timeToLive = customDuration ?? duration;
        StartCoroutine(AutoDestroy(toastGO, timeToLive));
    }

    private GameObject GetPrefabByType(ToastType type)
    {
        return type switch
        {
            ToastType.Info => toastInfoPrefab,
            ToastType.Success => toastSuccessPrefab,
            ToastType.Warning => toastWarningPrefab,
            ToastType.Error => toastErrorPrefab,
            _ => null
        };
    }

    private IEnumerator AutoDestroy(GameObject toastGO, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (toastGO != null)
            Destroy(toastGO);
    }
}
