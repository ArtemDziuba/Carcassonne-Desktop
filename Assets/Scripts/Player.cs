using UnityEngine;

public class Player
{
    public int PlayerId;
    public int MeepleSpriteIndex;

    public Player(int id, int spriteIndex)
    {
        PlayerId = id;
        MeepleSpriteIndex = spriteIndex;
    }
}
