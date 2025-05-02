using System.Collections.Generic;

[System.Serializable]
public class Segment
{
    public int Id; // ��������� ID � ����� �����
    public TerrainType Type;
    public List<int> ConnectedSegmentIds = new List<int>();

    public bool HasMeeple = false;
    public Player MeepleOwner = null;
}
