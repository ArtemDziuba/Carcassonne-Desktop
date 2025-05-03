using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Tile tilePrefab;
    public TileData startTileData; // Наприклад: Tile_1_city_road_straight
    public Board board;

    void Start()
    {
        Screen.SetResolution(1980, 1080, false); // Віконний режим

        // 1. Створюємо стартовий тайл
        Tile startTile = Instantiate(tilePrefab);
        startTile.Data = startTileData;
        startTile.SpriteRenderer = startTile.GetComponent<SpriteRenderer>();
        startTile.SpriteRenderer.sprite = startTileData.TileSprite;

        // 2. Ставимо його в центр дошки
        Vector2Int center = Vector2Int.zero;
        board.PlaceTile(center, startTile);

        // 3. Генеруємо shadow tiles довкола
        board.GenerateShadowsAround(center);
    }

    void Update()
    {
        // (Можна додати загальну логіку гри тут)
    }
}
