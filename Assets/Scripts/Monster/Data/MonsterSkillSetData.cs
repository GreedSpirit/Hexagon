using System;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// MonsterSkillSet 테이블의 데이터를 불러오는 클래스
/// </summary>
public class MonsterSkillSetData : CSVLoad, TableKey
{
    public int Id { get; set; }
    public string Key { get; set; }
    public MonsterGrade MonGrade { get; set; }
    public string SkillSlot1 { get; set; }
    public float Slot1Weight { get; set; }
    public string SkillSlot2 { get; set; }
    public float Slot2Weight { get; set; }
    public string SkillSlot3 { get; set; }
    public float Slot3Weight { get; set; }
    public string SkillSlot4 { get; set; }
    public float Slot4Weight { get; set; }

    public List<(string, float)> skillWeights = new List<(string, float)>();

    public void LoadFromCsv(string[] values)
    {
        // 0: Id
        if (int.TryParse(values[0], out int idValue))
            Id = idValue;
        else
            Id = 0;

        // 1: Key
        Key = values[1];

        // 2: MonGrade
        if (Enum.TryParse(values[2], out MonsterGrade gradeValue))
            MonGrade = gradeValue;
        else
            MonGrade = MonsterGrade.Normal;

        // 3: SkillSlot1
        SkillSlot1 = values[3];
        if(SkillSlot1 == "")
        {
            Debug.Log("SkillSlot1이 비어있습니다. Id:" + Id);
        }

        // 4: Slot1Weight
        if (float.TryParse(values[4], out float slot1WeightValue))
            Slot1Weight = slot1WeightValue;
        else
        {
            Debug.LogError("Slot1Weight 파싱 실패. Id:" + Id);
            Slot1Weight = 0f;
        }

        // 5: SkillSlot2
        SkillSlot2 = values[5];
        if(SkillSlot2 == "")
        {
            Debug.Log("SkillSlot2이 비어있습니다. Id:" + Id);
        }

        // 6: Slot2Weight
        if (float.TryParse(values[6], out float slot2WeightValue))
            Slot2Weight = slot2WeightValue;
        else
        {
            Debug.LogError("Slot2Weight 파싱 실패. Id:" + Id);
            Slot2Weight = 0f;
        }

        // 7: SkillSlot3
        SkillSlot3 = values[7];
        if(MonGrade == MonsterGrade.Boss && SkillSlot3 == "")
        {
            Debug.Log("보스 몬스터의 SkillSlot3이 비어있습니다. Id:" + Id);
        }

        // 8: Slot3Weight
        if (float.TryParse(values[8], out float slot3WeightValue))
            Slot3Weight = slot3WeightValue;
        else
        {
            Debug.LogError("Slot3Weight 파싱 실패. Id:" + Id);
            Slot3Weight = 0f;
        }

        // 9: SkillSlot4
        SkillSlot4 = values[9];
        if(MonGrade == MonsterGrade.Boss && SkillSlot4 == "")
        {
            Debug.Log("보스 몬스터의 SkillSlot4이 비어있습니다. Id:" + Id);
        }

        // 10: Slot4Weight
        if (float.TryParse(values[10], out float slot4WeightValue))
            Slot4Weight = slot4WeightValue;
        else
        {
            Debug.LogError("Slot4Weight 파싱 실패. Id:" + Id);
            Slot4Weight = 0f;
        }

        if(SkillSlot4 != "" && MonGrade == MonsterGrade.Normal)
        {
            Debug.Log("일반 몬스터의 SkillSlot4가 비어있지 않습니다. Id:" + Id);
            Slot4Weight = 0f;
        }

        if(SkillSlot1 != "")
            skillWeights.Add((SkillSlot1, Slot1Weight));
        if(SkillSlot2 != "")
            skillWeights.Add((SkillSlot2, Slot2Weight));
        if(SkillSlot3 != "")
            skillWeights.Add((SkillSlot3, Slot3Weight));
        if(SkillSlot4 != "")
            skillWeights.Add((SkillSlot4, Slot4Weight));
    }
}
