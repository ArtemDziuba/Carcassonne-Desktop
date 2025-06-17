using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Клас, що відповідає за збереження даних сегментів тайлів
[Serializable]
public class SegmentSaveData
{
    public int id;
    public bool hasMeeple;
    public int meepleOwnerId;
}