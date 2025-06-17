// TileDeckManager.cs
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

// Клас, що відповідає за колоду тайлів
public class TileDeckManager : MonoBehaviour
{
    public List<TileData> uniqueTiles; // 24 asset-и
    public List<int> tileCounts;       // Кількість кожного
    public List<TileData> fullDeck;

    void Awake()
    {
        fullDeck = new List<TileData>();
        for (int i = 0; i < uniqueTiles.Count; i++)
            for (int j = 0; j < tileCounts[i]; j++)
                fullDeck.Add(uniqueTiles[i]);

        Shuffle(fullDeck);
    }

    void Shuffle(List<TileData> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int r = Random.Range(i, list.Count);
            (list[i], list[r]) = (list[r], list[i]);
        }
    }

    public void ReturnTile(Tile returnedTile)
    {
        int insertIndex = Random.Range(1, fullDeck.Count + 1);
        fullDeck.Insert(insertIndex, returnedTile.Data);
    }

    /// <summary>
    /// Дістане верхню карту й видалить її з колоди.
    /// </summary>
    public TileData DrawTile()
    {
        if (fullDeck.Count == 0) return null;
        var res = fullDeck[0];
        fullDeck.RemoveAt(0);
        return res;
    }

    /// <summary>
    /// Чи порожня зараз колода?
    /// </summary>
    public bool IsEmpty()
        => fullDeck.Count == 0;

    /// <summary>
    /// Скільки тайлів ще залишилось у колоді?
    /// </summary>
    public int TilesRemaining
        => fullDeck.Count;
}
