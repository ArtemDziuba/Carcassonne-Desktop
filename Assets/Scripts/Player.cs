public class Player
{
    public int PlayerId { get; private set; }
    public int MeepleSpriteIndex { get; private set; }
    public int MeepleCount { get; private set; } = 7;
    public int Score { get; set; } = 0;

    public string Name { get; private set; }

    public Player(int id, int spriteIndex, string name = null)
    {
        PlayerId = id;
        MeepleSpriteIndex = spriteIndex;
        Name = string.IsNullOrWhiteSpace(name) ? $"ֳנאגוצü {id + 1}" : name;
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
