using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SkillSetData : CSVLoad, TableKey
{

    // 엑셀 컬럼명과 일치시킨 프로퍼티

    public int Id { get; set; }
    public string SkillSetKey { get; set; } // 엑셀 컬럼명: SkillSetKey
    public string Monster { get; set; }     // 몬스터 Key 참조
    public string Skill { get; set; }       // 스킬 Key 참조
    public int Rate { get; set; }           // 가중치 (확률)

    // DataManager가 'Key'로 접근하면 'SkillSetKey'를 반환
    string TableKey.Key
    {
        get { return SkillSetKey; }
    }


    // CSV 데이터 파싱
    public void LoadFromCsv(string[] values)
    {
        // 0: Id (int)
        if (int.TryParse(values[0], out int idValue))
        {
            Id = idValue;
        }
        else
        {
            Id = 0;
        }

        // 1: SkillSetKey (string)
        SkillSetKey = values[1];

        // 2: Monster (string)
        Monster = values[2];

        // 3: Skill (string)
        Skill = values[3];

        // 4: Rate (int)
        if (int.TryParse(values[4], out int rateValue))
        {
            Rate = rateValue;
        }
        else
        {
            Rate = 0;
        }
    }
}