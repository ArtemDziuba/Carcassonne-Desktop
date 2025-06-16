using UnityEngine;
using System.Collections.Generic;

// Клас, що відповідає за встановлення даних гри
public class GameConfig : MonoBehaviour
{
    public static GameConfig Instance;

    public List<Player> Players = new();
    public bool IsFieldEnabled { get; set; } = true;

    public float zoomSpeed = 100f;
    public float panSpeed = 5f;
    public float volume = 1f;

    private void Start()
    {
        if (Instance != null)
            return;

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
