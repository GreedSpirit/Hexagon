using UnityEngine;

[System.Serializable]
public class StageData : CSVLoad, TableKey
{

    // 엑셀 컬럼명과 일치시킨 프로퍼티
    public int Id { get; set; }
    public string StageKey { get; set; }       // 엑셀 컬럼명: StageKey (Key 역할)
    public string Dungeon { get; set; }         // 소속 던전 Key
    public string SpawnMonster { get; set; }    // 소환될 몬스터 Key
    public string Img { get; set; }
    public string Bgm { get; set; }

    string TableKey.Key   // 엑셀 컬럼명: StageName (Key 역할)
    {
        get { return StageKey; }
    }


    // CSV 데이터 파싱
    public void LoadFromCsv(string[] values)
    {
        // 0: Id
        if (int.TryParse(values[0], out int idValue)) Id = idValue;
        else Id = 0;

        // 1: StageKey
        StageKey = values[1];

        // 2: Dungeon
        Dungeon = values[2];

        // 3: SpawnMonster
        SpawnMonster = values[3];

        // 4: Img
        Img = values[4];

        // 5: Bgm
        Bgm = values[5];
    }
}