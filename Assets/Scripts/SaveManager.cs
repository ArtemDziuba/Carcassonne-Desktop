using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject saveEntryPrefab;
    [SerializeField] private Transform contentParent;
    [SerializeField] private Sprite defaultThumbnail;

    private string saveFolder;

    private void Start()
    {
        saveFolder = Path.Combine(Application.persistentDataPath, "Saves");

        if (!Directory.Exists(saveFolder))
            Directory.CreateDirectory(saveFolder);

        LoadAllSaves();
    }

    private void LoadAllSaves()
    {
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        string[] saveFiles = Directory.GetFiles(saveFolder, "*.dat");

        foreach (string filePath in saveFiles)
        {
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            GameSnapshot snapshot = LoadSnapshot(filePath);

            GameObject entryGO = Instantiate(saveEntryPrefab, contentParent);
            var entryUI = entryGO.GetComponent<SaveEntryUI>();

            string displayName = snapshot.saveName;
            if (string.IsNullOrWhiteSpace(displayName))
                displayName = $"Збереження {snapshot.date}";

            entryUI.Initialize(
                id: fileName,
                saveName: displayName,
                playerCount: snapshot.players?.Count ?? 0,
                thumbnail: defaultThumbnail,
                onLoad: OnLoadSave,
                onDelete: OnDeleteSave
            );
        }
    }

    private GameSnapshot LoadSnapshot(string path)
    {
        byte[] encrytpetJson = File.ReadAllBytes(path);

        AES crypto = new AES();
        string json = crypto.Decrypt(encrytpetJson);

        return JsonUtility.FromJson<GameSnapshot>(json);
    }

    private void OnLoadSave(string saveId)
    {
        GameSnapshot snapshot = SaveSystem.LoadGame(saveId);

        if (snapshot == null)
        {
            ToastManager.Instance?.ShowToast(ToastType.Error, "Не вдалося завантажити збереження.");
            return;
        }

        // Зберігаємо snapshot тимчасово для наступної сцени
        TempGameData.snapshotToLoad = snapshot;

        // Завантажуємо сцену гри
        SceneManager.LoadScene("MainGame");
    }

    private void OnDeleteSave(string saveId)
    {
        string path = Path.Combine(saveFolder, $"{saveId}.dat");
        if (File.Exists(path))
            File.Delete(path);

        LoadAllSaves(); // оновити список
    }
}
