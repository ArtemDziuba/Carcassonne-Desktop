using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameLoader : MonoBehaviour
{
    [Header("References")]
    public Board board;
    public Tile tilePrefab;
    public PlayerManager playerManager;
    public TileDeckManager deckManager;
    public PlayerUIManager uiManager;
    public TurnManager turnManager;
    public MeepleSpawner meepleSpawner;

    [Header("Tile Data Source")]
    public List<TileData> allTileData;

    private Dictionary<string, TileData> tileDataMap;

    void Awake()
    {
        tileDataMap = new Dictionary<string, TileData>();
        foreach (var data in allTileData)
        {
            if (!tileDataMap.ContainsKey(data.name))
                tileDataMap[data.name] = data;
        }
    }

    void Start()
    {
        if (TempGameData.snapshotToLoad != null)
        {
            LoadFromSnapshot(TempGameData.snapshotToLoad);
            TempGameData.snapshotToLoad = null;
        }
        else
        {
            Debug.LogWarning("Збереження не вибрано — запускається нова гра.");
        }
    }

    public void LoadFromSnapshot(GameSnapshot snapshot)
    {
        Debug.Log($"[LOAD] Починаємо відновлення. PlacedTiles={snapshot.placedTiles.Count}");

        // 0) Очищення старого стану
        foreach (var old in board.placedTiles.Values.ToList())
            old.ClearMeepleSlots();
        board.placedTiles.Clear();
        board.shadowTiles.Clear();

        // 1) Відновлюємо гравців
        var players = snapshot.players
            .Select(p => {
                var pl = new Player(p.id, p.spriteIndex, p.name)
                {
                    Score = p.score,
                    MeepleCount = p.meepleCount
                };
                return pl;
            })
            .ToList();

        GameConfig.Instance.Players = players;
        GameConfig.Instance.IsFieldEnabled = snapshot.isFieldEnabled;
        playerManager.SetPlayers(players);
        uiManager.InitializeUI(players);
        playerManager.SetCurrentPlayerIndex(snapshot.currentPlayerIndex);

        // 2) Відновлюємо плитки
        foreach (var placed in snapshot.placedTiles)
        {
            Debug.Log($"[LOAD] Відновлюємо '{placed.tileName}' @ {placed.position} (segmentsWithMeeples: {placed.segmentData.Count})");

            // 2.1) Знаходимо TileData
            if (!tileDataMap.TryGetValue(placed.tileName, out var data))
            {
                Debug.LogError($"TileData '{placed.tileName}' не знайдено!");
                continue;
            }

            // 2.2) Instantiate + Initialize
            var tile = Instantiate(tilePrefab);
            tile.Initialize(data);
            tile.ClearMeepleSlots();

            // 2.3) Відновлюємо ротацію
            int turns = (placed.rotation / 90) % 4;
            for (int i = 0; i < turns; i++)
                tile.RotateClockwise();

            // 2.4) Ставимо на дошку (створює тіні й слоти)
            board.PlaceTile(placed.position, tile);

            // 2.5) Монастир, якщо є
            if (placed.hasMonasteryMeeple && placed.monasteryMeepleOwnerId >= 0)
            {
                var owner = players.First(p => p.PlayerId == placed.monasteryMeepleOwnerId);
                Vector3 monPos = tile.transform.position;
                monPos.z -= 0.1f;
                var monGO = Instantiate(meepleSpawner.meeplePrefab, monPos, Quaternion.identity);
                var monSR = monGO.GetComponent<SpriteRenderer>();
                monSR.sprite = meepleSpawner.meepleSprites[owner.MeepleSpriteIndex];
                monSR.sortingOrder = tile.GetComponent<SpriteRenderer>().sortingOrder + 1;
                tile.PlaceMonasteryMeeple(owner, monGO);
            }

            // 2.6) Відновлюємо всі інші міпли по segmentData
            var slots = tile.GetComponentsInChildren<MeeplePlacementSlot>();
            var segments = tile.GetSegments();

            foreach (var segSave in placed.segmentData)
            {
                if (!segSave.hasMeeple || segSave.meepleOwnerId < 0)
                    continue;

                var owner = players.First(p => p.PlayerId == segSave.meepleOwnerId);

                // знаходимо слот, що покриває цей сегмент
                var slot = slots.FirstOrDefault(s => s.CoveredSegments.Contains(segSave.id));
                if (slot == null)
                {
                    Debug.LogWarning($"[LOAD] Слот для сегменту {segSave.id} не знайдено на '{placed.tileName}'");
                    continue;
                }

                // якщо цей слот уже обробили — пропускаємо
                if (slot.IsOccupied)
                    continue;

                // спавнимо міпл трохи над плиткою
                Vector3 pos = slot.transform.position;
                pos.z -= 0.1f;
                var meepleGO = Instantiate(meepleSpawner.meeplePrefab, pos, Quaternion.identity);
                var sr = meepleGO.GetComponent<SpriteRenderer>();
                sr.sprite = meepleSpawner.meepleSprites[owner.MeepleSpriteIndex];
                sr.sortingOrder = tile.GetComponent<SpriteRenderer>().sortingOrder + 1;

                // позначаємо слот
                slot.IsOccupied = true;
                slot.MeepleOwner = owner;
                slot.CurrentMeeple = meepleGO;

                // і оновлюємо модель усіх сегментів цього слоту
                foreach (int segId in slot.CoveredSegments)
                {
                    var seg = segments[segId];
                    seg.HasMeeple = true;
                    seg.MeepleOwner = owner;
                    seg.MeepleObject = meepleGO;
                }
            }
        }

        // 3) Відновлюємо колоду
        deckManager.fullDeck = snapshot.remainingTiles
            .Select(name => tileDataMap[name])
            .ToList();

        // 4) Оновлюємо UI та лічильник деки
        uiManager.InitializeUI(players);
        turnManager.UpdateDeckCountUI();
        uiManager.SetActivePlayer(playerManager.CurrentPlayerIndex);

        // 5) Відновлюємо фазу ходу
        turnManager.RestorePhase(snapshot.tilePlaced, snapshot.meeplePlaced);

        Debug.Log($"Гру «{snapshot.saveName}» успішно відновлено.");
    }

}