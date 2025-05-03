using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Tile tilePrefab;
    public TileData startTileData;
    public Board board;

    void Start()
    {
        Screen.SetResolution(1980, 1080, false);

        Tile startTile = Instantiate(tilePrefab);
        startTile.Initialize(startTileData);

        Vector2Int center = Vector2Int.zero;
        board.PlaceTile(center, startTile);
        board.GenerateShadowsAround(center);
    }
}
