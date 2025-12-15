using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterStatData : CSVLoad, TableKey
{

    // 엑셀 컬럼명과 일치시킨 프로퍼티
    public int Level { get; set; }      // 0번: Level
    public int Hp { get; set; }         // 1번: Hp
    public int Defense { get; set; }    // 2번: Defense

    // 이 테이블도 'Level'이 곧 'Id'입니다.
    int TableKey.Id
    {
        get { return Level; }
    }

    // 레벨을 문자열로 변환해 Key로 사용합니다.
    string TableKey.Key
    {
        get { return Level.ToString(); }
    }

    // CSV 데이터 파싱
    public void LoadFromCsv(string[] values)
    {
        // 0: Level (int)
        if (int.TryParse(values[0], out int levelValue))
            Level = levelValue;
        else
            Level = 0;

        // 1: Hp (int)
        if (int.TryParse(values[1], out int hpValue))
            Hp = hpValue;
        else
            Hp = 0;

        // 2: Defense (int)
        if (int.TryParse(values[2], out int defValue))
            Defense = defValue;
        else
            Defense = 0;
    }
}