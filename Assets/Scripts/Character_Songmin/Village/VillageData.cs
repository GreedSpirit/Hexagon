
using UnityEngine;

[System.Serializable]
public class VillageData : CSVLoad, TableKey
{
    public int Id { get; set; }
    public string Key { get; set; }
    public string Name { get; set; }
    public float CharSpawnAreaX { get; set; }
    public float CharSpawnAreaY { get; set; }
    public float DungeonEntranceArea { get; set; }
    public float DungeonEntranceEndArea { get; set; }
    public string Img { get; set; }
    public string Bgm { get; set; }
    string TableKey.Key   // ¿¢¼¿ ÄÃ·³¸í: StageName (Key ¿ªÇÒ)
    {
        get { return Key; }
    }

    public void LoadFromCsv(string[] values)
    {
        // ÆÄ½Ì¿ë ÀÓ½Ã º¯¼ö
        int vInt;
        float vFloat;

        // 0: Id
        if (int.TryParse(values[0], out vInt))
        {
            Id = vInt;
        }
        else
        {
            Id = 0;
        }

        // 1 :  Key
        Key = values[1];

        // 2 :  Name
        Name = values[2];

        // 3 :  CharSpawnAreaX
        if (float.TryParse(values[3], out vFloat))
        {
            CharSpawnAreaX = vFloat;
        }
        else
        {
            CharSpawnAreaX = 0;
        }

        // 4 :  CharSpawnAreaY
        if (float.TryParse(values[4], out vFloat))
        {
            CharSpawnAreaY = vFloat;
        }
        else
        {
            CharSpawnAreaY = 0;
        }

        // 5 :  CharSpawnAreaX
        if (float.TryParse(values[5], out vFloat))
        {
            DungeonEntranceArea = vFloat;
        }
        else
        {
            DungeonEntranceArea = 0;
        }

        // 6 :  DungeonEntranceEndArea
        if (float.TryParse(values[6], out vFloat))
        {
            DungeonEntranceEndArea = vFloat;
        }
        else
        {
            DungeonEntranceEndArea = 0;
        }

        // 7 :  Img
        Img = values[7];

        // 8 :  Bgm
        Bgm = values[8];
    }
}
