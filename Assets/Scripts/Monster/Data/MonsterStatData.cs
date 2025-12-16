using UnityEngine;

[System.Serializable]
public class MonsterStatData : CSVLoad, TableKey
{
    public int Level { get; set; }
    public int Hp { get; set; }
    public int Defense { get; set; }

    int TableKey.Id => Level; // Level을 Id로 사용
    string TableKey.Key => Level.ToString(); // string 값이 없기 때문에 Level을 문자열로 변환하여 사용

    public void LoadFromCsv(string[] values)
    {
        // 0: Level
        if (int.TryParse(values[0], out int levelValue))
            Level = levelValue;
        else
            Level = 1;

        // 1: Hp
        if (int.TryParse(values[1], out int hpValue))
            Hp = hpValue;
        else
        {
            Debug.LogError("Hp 파싱 실패. Level:" + Level);
            Hp = 100;
        }

        // 2: Defense
        if (int.TryParse(values[2], out int defValue))
            Defense = defValue;
        else
        {
            Debug.LogError("Defense 파싱 실패. Level:" + Level);
            Defense = 0;
        }
    }
}
