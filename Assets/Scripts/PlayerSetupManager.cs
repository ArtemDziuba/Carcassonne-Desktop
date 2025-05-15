using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerSetupManager : MonoBehaviour
{
    public int playerCount = 2;
    public GameObject playerSetupPrefab;
    public Transform contentParent;
    public List<Sprite> meepleSprites;

    private List<GameObject> createdPlayers = new();

    void Start()
    {
        GeneratePlayerSetups();
    }

    void GeneratePlayerSetups()
    {
        Debug.Log($"MeepleSprites у SetupManager: {meepleSprites.Count}");
        for (int i = 0; i < playerCount; i++)
        {
            GameObject go = Instantiate(playerSetupPrefab, contentParent);

            var selector = go.GetComponent<MeepleSelector>();
            selector.meepleSprites = meepleSprites; 

            createdPlayers.Add(go);
            Debug.Log($"MeepleSprites у Selector[{i}]: {selector.meepleSprites.Count}");
        }
    }

    public void OnStartGame()
    {
        GameConfig.Instance.Players.Clear();

        for (int i = 0; i < createdPlayers.Count; i++)
        {
            var selector = createdPlayers[i].GetComponent<MeepleSelector>();
            var inputField = createdPlayers[i].GetComponentInChildren<InputField>();

            int meepleIndex = meepleSprites.IndexOf(selector.MeepleImageTarget.sprite);
            if (meepleIndex == -1) meepleIndex = 0; // fallback

            Player player = new Player(i, meepleIndex);
            GameConfig.Instance.Players.Add(player);

            // Опціонально: збережи ім’я (якщо потрібно)
            if (!string.IsNullOrWhiteSpace(inputField.text))
                player.GetType().GetProperty("Name")?.SetValue(player, inputField.text);
        }

        SceneManager.LoadScene("MainGame");
    }
}
