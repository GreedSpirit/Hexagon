using UnityEngine;

[System.Serializable]
public class NpcData : CSVLoad, TableKey
{
    public int Id { get; set; }
    public string Village { get; set; }
    public string Npc { get; set; }

    public float NpcAreaX { get; set; }
    public float NpcAreaY { get; set; }

    string TableKey.Key   // ¿¢¼¿ ÄÃ·³¸í: StageName (Key ¿ªÇÒ)
    {
        get { return Npc; }
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

        // 1 :  Village
        Village = values[1];

        // 2 :  Npc
        Npc = values[2];

        // 3: NpcAreaX
        if (float.TryParse(values[3], out vFloat))
        {
            NpcAreaX = vFloat;
        }
        else
        {
            NpcAreaX = 0;
        }

        // 4: NpcAreaY
        if (float.TryParse(values[4], out vFloat))
        {
            NpcAreaY = vFloat;
        }
        else
        {
            NpcAreaY = 0;
        }
    }
}
