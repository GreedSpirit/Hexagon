using System;
using UnityEngine;

/// <summary>
/// Monster 테이블의 데이터를 불러오는 클래스
/// </summary>
public enum MonsterGrade
{
    Normal,
    Boss
}

public class MonsterData : CSVLoad, TableKey
{
    public int Id { get; set; }
    public string Key { get; set; }
    public string Name { get; set; }
    public string Desc { get; set; }
    public MonsterGrade MonGrade { get; set; }
    public float HpRate { get; set; }
    public float DefRate { get; set; }
    public float MoveSpeed { get; set; }
    public string SkillSet { get; set; }
    public string Model { get; set; }

    public void LoadFromCsv(string[] values)
    {
        // 0: Id
        if (int.TryParse(values[0], out int idValue))
            Id = idValue;
        else
            Id = 0;

        // 1: Key
        Key = values[1];

        // 2: Name
        Name = values[2];
        if(Name == "")
        {
            Debug.Log("Name이 비어있습니다. Id:" + Id);
            Name = "???";
        }

        // 3: Desc
        Desc = values[3];
        if(Desc == "")
        {
            Debug.Log("Desc가 비어있습니다. Id:" + Id);
            Desc = "???";
        }

        // 4: MonGrade
        if (Enum.TryParse(values[4], out MonsterGrade gradeValue))
            MonGrade = gradeValue;
        else
            MonGrade = MonsterGrade.Normal;

        // 5: HpRate
        if (float.TryParse(values[5], out float hpRateValue))
            HpRate = hpRateValue;
        else
            HpRate = 1.0f;

        // 6: DefRate
        if (float.TryParse(values[6], out float defRateValue))
            DefRate = defRateValue;
        else
            DefRate = 1.0f;

        // 7: MoveSpeed
        if (float.TryParse(values[7], out float moveSpeedValue))
            MoveSpeed = moveSpeedValue;
        else
            MoveSpeed = 10.0f;
        // 8: SkillSet
        SkillSet = values[8];
        if(SkillSet == "")
        {
            Debug.LogError("SkillSet이 비어있습니다. Id:" + Id);
        }

        // 9: Model
        Model = values[9];
    }
}
