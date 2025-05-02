using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Tile tilePrefab;
    public TileData startTileData; // Tile_1_city_road_straight
    public Board board;

    void Start()
    {
        Screen.SetResolution(1980, 1080, false); // Віконний режим 1980x1080

        // Створюємо стартовий тайл у центрі поля
        Tile startTile = Instantiate(tilePrefab);
        startTile.Data = startTileData;
        startTile.SpriteRenderer = startTile.GetComponent<SpriteRenderer>();
        startTile.SpriteRenderer.sprite = startTileData.TileSprite;

        board.PlaceTile(Vector2Int.zero, startTile);
    }

    void Update()
    {
        // Логіка гри кожен кадр (тимчасово порожня)
    }
}
