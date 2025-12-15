using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterLevelData : CSVLoad, TableKey
{

    // 엑셀 컬럼명과 일치시킨 프로퍼티
    public int Level { get; set; }      // 0번: Level
    public int NeedExp { get; set; }    // 1번: NeedExp

    // 'Level'로 'Id'를 대체했습니다.
    int TableKey.Id
    {
        get { return Level; }
    }

    // 별도의 Key 문자열이 없으므로, 레벨을 문자열로 변환해 사용합니다.
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

        // 1: NeedExp (int)
        if (int.TryParse(values[1], out int expValue))
            NeedExp = expValue;
        else
            NeedExp = 0;
    }
}