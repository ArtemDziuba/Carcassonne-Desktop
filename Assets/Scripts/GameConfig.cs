using UnityEngine;
using System.Collections.Generic;

public class GameConfig : MonoBehaviour
{
    public static GameConfig Instance;

    public List<Player> Players = new();
    public bool IsFieldEnabled { get; set; } = true;

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
