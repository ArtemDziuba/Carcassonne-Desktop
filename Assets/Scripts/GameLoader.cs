using System.Collections.Generic;
using System.Linq;
using Unity.Burst.CompilerServices;
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
            .Select(p =>
            {
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

        // 2) Фільтруємо placedTiles — беремо лише останню ітерацію для кожної позиції + імені тайла
        var uniquePlacements = snapshot.placedTiles
            .GroupBy(p => new { p.position, p.tileName })
            .Select(g => g.Last())
            .ToList();

        // 3) Відновлюємо плитки
        foreach (var placed in uniquePlacements)
        {
            Debug.Log($"[LOAD] Відновлюємо '{placed.tileName}' @ {placed.position} (segmentsWithMeeples: {placed.segmentData.Count})");

            // 3.1) Знаходимо TileData
            if (!tileDataMap.TryGetValue(placed.tileName, out var data))
            {
                Debug.LogError($"TileData '{placed.tileName}' не знайдено!");
                continue;
            }

            // 3.2) Instantiate + Initialize
            var tile = Instantiate(tilePrefab);
            tile.Initialize(data);
            tile.ClearMeepleSlots();

            // 3.3) Відновлюємо ротацію
            int turns = (placed.rotation / 90) % 4;
            for (int i = 0; i < turns; i++)
                tile.RotateClockwise();

            // 3.4) Ставимо на дошку (створює тіні й слоти)
            board.PlaceTile(placed.position, tile);

            // 3.5) Відновлюємо монастиря, якщо є
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

            // 3.6) Відновлюємо всі інші міпли по segmentData
            var slots = tile.GetComponentsInChildren<MeeplePlacementSlot>().Reverse();
            var segments = tile.GetSegments();

            var segIds = segments.Select(s => s.Id).ToList();
            //Debug.Log($"[LOAD][{placed.tileName}@{placed.position}] rotation={placed.rotation}, segments after rotate: [{string.Join(",", segIds)}]");  // <<< LOG HERE

            //foreach (var slot in slots)
            //{
            //    Debug.Log($"[LOAD][{placed.tileName}] Slot covers segments: [{string.Join(",", slot.CoveredSegments)}]");  // <<< LOG HERE
            //}

            foreach (var segSave in placed.segmentData)
            {
                if (!segSave.hasMeeple || segSave.meepleOwnerId < 0)
                    continue;
;
                var slot = slots.FirstOrDefault(s => s.CoveredSegments.Contains(segSave.id));
                if (slot == null || slot.IsOccupied)
                    continue;
                //Debug.Log($"[LOAD][{placed.tileName}] Slot SELECTED segments: [{string.Join(",", slot.CoveredSegments)}]");

                var owner = players.First(p => p.PlayerId == segSave.meepleOwnerId);

                // Обчислюємо світову позицію з урахуванням обертання тайла
                Vector3 localPos = slot.transform.localPosition;
                if(placed.rotation == 90 || placed.rotation == 270)
                {
                    localPos.x *= -1;
                    localPos.y *= -1;
                }
                Vector3 worldPos = tile.transform.position + (Quaternion.Euler(0, 0, placed.rotation) * localPos);
                worldPos.z = tile.transform.position.z - 0.1f;
                //Debug.Log($"[LOAD][{placed.tileName}] localPos: [{localPos}], worldPos: [{worldPos}]");

                var meepleGO = Instantiate(meepleSpawner.meeplePrefab, worldPos, Quaternion.identity);
                var sr = meepleGO.GetComponent<SpriteRenderer>();
                sr.sprite = meepleSpawner.meepleSprites[owner.MeepleSpriteIndex];
                sr.sortingOrder = tile.GetComponent<SpriteRenderer>().sortingOrder + 1;

                // Позначаємо слот і оновлюємо модель сегментів
                slot.IsOccupied = true;
                slot.MeepleOwner = owner;
                slot.CurrentMeeple = meepleGO;

                foreach (int segId in slot.CoveredSegments)
                {
                    var seg = segments[segId];
                    seg.HasMeeple = true;
                    seg.MeepleOwner = owner;
                    seg.MeepleObject = meepleGO;
                }
            }
        }

        // 4) Відновлюємо колоду
        deckManager.fullDeck = snapshot.remainingTiles
            .Select(name => tileDataMap[name])
            .ToList();

        // 5) Оновлюємо UI та лічильник деки
        uiManager.InitializeUI(players);
        turnManager.UpdateDeckCountUI();
        uiManager.SetActivePlayer(playerManager.CurrentPlayerIndex);

        // 6) Відновлюємо фазу ходу
        turnManager.RestorePhase(snapshot.tilePlaced, snapshot.meeplePlaced);

        Debug.Log($"Гру «{snapshot.saveName}» успішно відновлено.");
        ToastManager.Instance.ShowToast(ToastType.Success,
                $"Гру «{snapshot.saveName}» успішно відновлено.");
    }

}