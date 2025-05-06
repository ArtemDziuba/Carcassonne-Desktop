public class Player
{
    public int PlayerId { get; private set; }
    public int MeepleSpriteIndex { get; private set; }
    public int MeepleCount { get; private set; } = 7;

    public Player(int id, int spriteIndex)
    {
        PlayerId = id;
        MeepleSpriteIndex = spriteIndex;
    }

    public bool HasMeeples() => MeepleCount > 0;

    public void UseMeeple()
    {
        if (MeepleCount > 0)
            MeepleCount--;
    }

    public void ReturnMeeple()
    {
        MeepleCount++;
    }
}
