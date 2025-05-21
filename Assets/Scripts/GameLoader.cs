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
        if (meepleSpawner == null) Debug.LogError("[LOAD] meepleSpawner не заданий!");
        if (meepleSpawner.meeplePrefab == null) Debug.LogError("[LOAD] meeplePrefab == null!");
        if (meepleSpawner.meepleSprites == null) Debug.LogError("[LOAD] meepleSprites == null!");

        // 0) Очищаємо все з попередньої гри
        foreach (var oldTile in board.placedTiles.Values.ToList())
            oldTile.ClearMeepleSlots();
        board.placedTiles.Clear();
        board.shadowTiles.Clear();

        // 1) Відновлюємо гравців
        List<Player> players = new List<Player>();
        foreach (var p in snapshot.players)
        {
            var player = new Player(p.id, p.spriteIndex, p.name)
            {
                Score = p.score,
                MeepleCount = p.meepleCount  // встановлюємо залишок міплів
            };
            players.Add(player);
        }
        GameConfig.Instance.Players = players;
        GameConfig.Instance.IsFieldEnabled = snapshot.isFieldEnabled;
        playerManager.SetPlayers(players);
        uiManager.InitializeUI(players);
        playerManager.SetCurrentPlayerIndex(snapshot.currentPlayerIndex);

        // 2) Відновлюємо плитки на полі
        foreach (var placed in snapshot.placedTiles)
        {
            Debug.Log($"[LOAD] Відновлюємо тайл '{placed.tileName}' @ {placed.position} — сегментів у даних: {placed.segmentData.Count}");
            if (!tileDataMap.TryGetValue(placed.tileName, out var data))
            {
                Debug.LogError($"TileData {placed.tileName} не знайдено.");
                continue;
            }

            // 2.1) Створюємо та ініціалізуємо тайл
            var tile = Instantiate(tilePrefab);
            tile.Initialize(data);
            tile.ClearMeepleSlots();  // прибираємо слоти, створені в Initialize

            // 2.2) Повертаємо ротацію
            int turns = (placed.rotation / 90) % 4;
            for (int i = 0; i < turns; i++)
                tile.RotateClockwise();

            // 2.3) Ставимо тайл на дошку (створює тіні й слоти)
            board.PlaceTile(placed.position, tile);

            // 2.4) Відновлюємо монастирного міпла
            if (placed.hasMonasteryMeeple && placed.monasteryMeepleOwnerId >= 0)
            {
                // Знаходимо власника
                var owner = players.First(p => p.PlayerId == placed.monasteryMeepleOwnerId);

                // Розташування — центр плитки
                Vector3 spawnPos = tile.transform.position;
                // Трохи піднімаємо по Z, щоб міпл точно малювався над плиткою
                spawnPos.z = tile.transform.position.z - 0.1f;

                // Створюємо GameObject міпла
                var monGO = Instantiate(meepleSpawner.meeplePrefab, spawnPos, Quaternion.identity);
                var sr = monGO.GetComponent<SpriteRenderer>();
                // Встановлюємо спрайт, sortingOrder вище за плитку
                sr.sprite = meepleSpawner.meepleSprites[owner.MeepleSpriteIndex];
                sr.sortingOrder = tile.GetComponent<SpriteRenderer>().sortingOrder + 1;

                // Логіка в самому тайлі
                tile.PlaceMonasteryMeeple(owner, monGO);
            }

            // 2.5) Відновлюємо всі інші сегментні міпли — по одному на кожен слот
            var slots = tile.GetComponentsInChildren<MeeplePlacementSlot>();
            var segments = tile.GetSegments();

            // Збираємо тільки ті збереження сегментів, де дійсно стояв міпл
            var segSaves = placed.segmentData
                .Where(s => s.hasMeeple && s.meepleOwnerId >= 0)
                .ToList();

            foreach (var slot in slots)
            {
                // перевіряємо, чи хоч один із saved-segments цього слота мав міпл
                var match = segSaves.FirstOrDefault(s => slot.CoveredSegments.Contains(s.id));
                if (match == null)
                    continue;

                // знаходимо гравця
                var owner = players.First(p => p.PlayerId == match.meepleOwnerId);

                // поза тим, де спавнити: центр цього слота, але на 0.1 вище Z тайлу
                Vector3 pos = slot.transform.position;
                pos.z = tile.transform.position.z - 0.1f;

                // створюємо один GameObject-міпл
                var meepleGO = Instantiate(meepleSpawner.meeplePrefab, pos, Quaternion.identity);

                // призначаємо спрайт і sortingOrder вище за плитку
                var sr = meepleGO.GetComponent<SpriteRenderer>();
                sr.sprite = meepleSpawner.meepleSprites[owner.MeepleSpriteIndex];
                sr.sortingOrder = tile.GetComponent<SpriteRenderer>().sortingOrder + 1;

                // оновлюємо слот і модель сегмента
                slot.IsOccupied = true;
                slot.MeepleOwner = owner;
                slot.CurrentMeeple = meepleGO;

                // прив’язуємо GameObject до кожного відповідного Segment у цьому слоті
                foreach (int segId in slot.CoveredSegments)
                {
                    var seg = segments[segId];
                    if (!seg.HasMeeple)
                    {
                        seg.HasMeeple = true;
                        seg.MeepleOwner = owner;
                        seg.MeepleObject = meepleGO;
                    }
                }
            }
        }

        // 3) Відновлюємо колоду
        deckManager.fullDeck = snapshot.remainingTiles
            .Select(name => tileDataMap[name])
            .ToList();

        // 4) Оновлюємо UI: гравці, кнопки, лічильник деки
        uiManager.InitializeUI(players);
        turnManager.UpdateDeckCountUI();
        uiManager.SetActivePlayer(playerManager.CurrentPlayerIndex);

        // 5) Відновлюємо фазу ходу
        turnManager.RestorePhase(snapshot.tilePlaced, snapshot.meeplePlaced);

        Debug.Log($"Гру «{snapshot.saveName}» успішно відновлено.");
    }

}