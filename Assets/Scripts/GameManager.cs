using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Tile tilePrefab;
    public TileData startTileData; // ���������: Tile_1_city_road_straight
    public Board board;

    void Start()
    {
        Screen.SetResolution(1980, 1080, false); // ³������ �����

        // 1. ��������� ��������� ����
        Tile startTile = Instantiate(tilePrefab);
        startTile.Data = startTileData;
        startTile.SpriteRenderer = startTile.GetComponent<SpriteRenderer>();
        startTile.SpriteRenderer.sprite = startTileData.TileSprite;

        // 2. ������� ���� � ����� �����
        Vector2Int center = Vector2Int.zero;
        board.PlaceTile(center, startTile);

        // 3. �������� shadow tiles �������
        board.GenerateShadowsAround(center);
    }

    void Update()
    {
        // (����� ������ �������� ����� ��� ���)
    }
}
