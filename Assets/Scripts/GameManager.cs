using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Tile tilePrefab;
    public TileData startTileData;
    public Board board;

    void Start()
    {
        Tile startTile = Instantiate(tilePrefab);
        startTile.Initialize(startTileData);

        Vector2Int center = Vector2Int.zero;
        board.PlaceTile(center, startTile);
        board.GenerateShadowsAround(center);
    }
}
