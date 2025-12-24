
using UnityEngine;

[System.Serializable]
public class NpcTalkData : CSVLoad, TableKey
{
    public int Id { get; set; }
    public string Npc { get; set; }
    public string NpcTalk1 { get; set; }
    public string NpcTalk2 { get; set; }
    public string NpcTalk3 { get; set; }
    public string NpcTalk4 { get; set; }
    string TableKey.Key   // ¿¢¼¿ ÄÃ·³¸í: StageName (Key ¿ªÇÒ)
    {
        get { return Npc; }
    }
    // CSV µ¥ÀÌÅÍ ÆÄ½Ì
    public void LoadFromCsv(string[] values)
    {
        // ÆÄ½Ì¿ë ÀÓ½Ã º¯¼ö
        int vInt;

        // 0: Id
        if (int.TryParse(values[0], out vInt))
        {
            Id = vInt;
        }
        else
        {
            Id = 0;
        }
        // 1 :  Npc
        Npc = values[1];

        // 2 :  NpcTalk1
        NpcTalk1 = values[2];

        // 3 :  NpcTalk2
        NpcTalk2 = values[3];

        // 4 :  NpcTalk3
        NpcTalk3 = values[4];

        // 5 :  NpcTalk4
        NpcTalk4 = values[5];

    }
}
