using UnityEngine;

public class GameManager : MonoBehaviour
{
    public TileSpawner tileSpawner;
    public TileData startingTileData;

    void Start()
    {
        Screen.SetResolution(1980, 1080, false);

        // Створення стартового тайла вручну
        var tile = Instantiate(tileSpawner.tilePrefab);
        tile.Data = startingTileData;
        tile.SpriteRenderer = tile.GetComponent<SpriteRenderer>();
        tile.SpriteRenderer.sprite = startingTileData.TileSprite;
        tile.transform.position = new Vector3(0, 0, 0);

        tileSpawner.board.PlaceTile(Vector2Int.zero, tile);
    }

    void Update()
    {
        // Логіка гри кожен кадр (тимчасово порожня)
    }
}