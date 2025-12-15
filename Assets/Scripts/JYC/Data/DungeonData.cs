using UnityEngine;

[System.Serializable]
public class DungeonData : CSVLoad, TableKey
{
    // ¿¢¼¿ ÄÃ·³¸í°ú ÀÏÄ¡½ÃÅ² ÇÁ·ÎÆÛÆ¼
    public int Id { get; set; }
    public string DungeonKey { get; set; }      // ¿¢¼¿ ÄÃ·³¸í: DungeonKey
    public string Name { get; set; }
    public string Desc { get; set; }
    public int RequiredLevel { get; set; }
    public int NumberOfStages { get; set; }
    public string Reward { get; set; }


    // ¿¢¼¿ ÄÃ·³¸í¿¡ ¸ÂÃç¼­ Key ¼³Á¤
    string TableKey.Key
    {
        get { return DungeonKey; }
    }


    // CSV µ¥ÀÌÅÍ ÆÄ½Ì
 
    public void LoadFromCsv(string[] values)
    {
        // 0: Id
        if (int.TryParse(values[0], out int idValue)) Id = idValue;
        else Id = 0;

        // 1: DungeonKey
        DungeonKey = values[1];

        // 2: Name
        Name = values[2];

        // 3: Desc
        Desc = values[3];

        // 4: RequiredLevel
        if (int.TryParse(values[4], out int reqLevelValue)) RequiredLevel = reqLevelValue;
        else RequiredLevel = 0;

        // 5: NumberOfStages
        if (int.TryParse(values[5], out int numStagesValue)) NumberOfStages = numStagesValue;
        else NumberOfStages = 0;

        // 6: Reward
        Reward = values[6];
    }
}