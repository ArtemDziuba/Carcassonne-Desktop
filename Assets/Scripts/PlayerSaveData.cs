    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Клас, що відповідає за збереження даних гравців
[Serializable]
public class PlayerSaveData
{
    public int id;
    public int spriteIndex;
    public int meepleCount;
    public int score;
    public string name;
}