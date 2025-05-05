using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public int playerCount = 2;
    public List<Player> Players = new();
    public int CurrentPlayerIndex { get; private set; } = 0;

    void Awake()
    {
        for (int i = 0; i < playerCount; i++)
            Players.Add(new Player(i, i)); // Meeple sprite index = i
    }

    public Player GetCurrentPlayer()
    {
        return Players[CurrentPlayerIndex];
    }

    public void NextPlayer()
    {
        CurrentPlayerIndex = (CurrentPlayerIndex + 1) % Players.Count;
    }
}
