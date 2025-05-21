using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public int playerCount = 2;
    public List<Player> Players { get; private set; } = new();
    public int CurrentPlayerIndex { get; private set; } = 0;

    /// <summary>
    /// ������� ����� ������� (����� ������ ������).
    /// </summary>
    public void CreatePlayers(int count)
    {
        playerCount = Mathf.Clamp(count, 2, 5); // ���������: 2�5 �������
        Players.Clear();

        for (int i = 0; i < playerCount; i++)
        {
            Players.Add(new Player(i, i)); // ID = i, MeepleSpriteIndex = i
        }

        CurrentPlayerIndex = 0;
    }

    public void SetPlayers(List<Player> players)
    {
        Players = new List<Player>(players);
        playerCount = Players.Count;
        CurrentPlayerIndex = 0;
    }

    public void SetCurrentPlayerIndex(int index)
    {
        if (index >= 0 && index < Players.Count)
            CurrentPlayerIndex = index;
    }

    /// <summary>
    /// �������� ��������� ������.
    /// </summary>
    public Player GetCurrentPlayer()
    {
        return Players[CurrentPlayerIndex];
    }

    /// <summary>
    /// ������� �� ���������� ������.
    /// </summary>
    public void NextPlayer()
    {
        CurrentPlayerIndex = (CurrentPlayerIndex + 1) % Players.Count;
    }
}
