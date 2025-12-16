using System;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// MonsterSkillSet 테이블의 데이터를 불러오는 클래스
/// </summary>

[System.Serializable]
public class MonsterSkillSetData : CSVLoad, TableKey
{
    public int Id { get; set; }
    public string Key { get; set; }
    public MonsterGrade MonGrade { get; set; }
    public string SkillSlot1 { get; set; }
    public float Slot1Weight { get; set; }
    public int Slot1Level { get; set; }
    public string SkillSlot2 { get; set; }
    public float Slot2Weight { get; set; }
    public int Slot2Level { get; set; }
    public string SkillSlot3 { get; set; }
    public float Slot3Weight { get; set; }
    public int Slot3Level { get; set; }
    public string SkillSlot4 { get; set; }
    public float Slot4Weight { get; set; }
    public int Slot4Level { get; set; }

    public List<(string, float)> skillWeights = new List<(string, float)>();
    public List<int> skillLevels = new List<int>();

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
        // 5: Slot1Level
        if (int.TryParse(values[5], out int slot1LevelValue))
            Slot1Level = slot1LevelValue;
        else
        {
            Debug.LogError("Slot1Level 파싱 실패. Id:" + Id);
            Slot1Level = 1;
        }
        // 6: SkillSlot2
        SkillSlot2 = values[6];
        if(SkillSlot2 == "")
        {
            Debug.Log("SkillSlot2이 비어있습니다. Id:" + Id);
        }
        // 7: Slot2Weight
        if (float.TryParse(values[7], out float slot2WeightValue))
            Slot2Weight = slot2WeightValue;
        else
        {
            Debug.LogError("Slot2Weight 파싱 실패. Id:" + Id);
            Slot2Weight = 0f;
        }
        // 8: Slot2Level
        if (int.TryParse(values[8], out int slot2LevelValue))
            Slot2Level = slot2LevelValue;
        else
        {
            Debug.LogError("Slot2Level 파싱 실패. Id:" + Id);
            Slot2Level = 1;
        }
        // 9: SkillSlot3
        SkillSlot3 = values[9];
        if(MonGrade == MonsterGrade.Boss && SkillSlot3 == "")
        {
            Debug.Log("SkillSlot3이 비어있습니다. Id:" + Id);
        }
        // 10: Slot3Weight
        if (float.TryParse(values[10], out float slot3WeightValue))
            Slot3Weight = slot3WeightValue;
        else
        {
            Debug.LogError("Slot3Weight 파싱 실패. Id:" + Id);
            Slot3Weight = 0f;
        }
        // 11: Slot3Level
        if (int.TryParse(values[11], out int slot3LevelValue))
            Slot3Level = slot3LevelValue;
        else
        {
            Debug.LogError("Slot3Level 파싱 실패. Id:" + Id);
            Slot3Level = 1;
        }
        // 12: SkillSlot4
        SkillSlot4 = values[12];
        if(MonGrade == MonsterGrade.Boss && SkillSlot4 == "")
        {
            Debug.Log("SkillSlot4이 비어있습니다. Id:" + Id);
        }
        // 13: Slot4Weight
        if (float.TryParse(values[13], out float slot4WeightValue))
            Slot4Weight = slot4WeightValue;
        else
        {
            Debug.LogError("Slot4Weight 파싱 실패. Id:" + Id);
            Slot4Weight = 0f;
        }
        // 14: Slot4Level
        if (int.TryParse(values[14], out int slot4LevelValue))
            Slot4Level = slot4LevelValue;
        else
        {
            Debug.LogError("Slot4Level 파싱 실패. Id:" + Id);
            Slot4Level = 1;
        }        
        // 일반 몬스터의 경우 SkillSlot4는 어떤 일이 있어도 들어올 수 없기에 경고 표시
        if(SkillSlot4 != "" && MonGrade == MonsterGrade.Normal)
        {
            Debug.LogError("일반 몬스터의 SkillSlot4가 비어있지 않습니다. Id:" + Id);
            Slot4Weight = 0f;
        }

        if(SkillSlot1 != ""){
            skillWeights.Add((SkillSlot1, Slot1Weight));
            skillLevels.Add(Slot1Level);
        }
        if(SkillSlot2 != ""){
            skillWeights.Add((SkillSlot2, Slot2Weight));
            skillLevels.Add(Slot2Level);
        }
        if(SkillSlot3 != ""){
            skillWeights.Add((SkillSlot3, Slot3Weight));
            skillLevels.Add(Slot3Level);
        }
        if(SkillSlot4 != ""){
            skillWeights.Add((SkillSlot4, Slot4Weight));
            skillLevels.Add(Slot4Level);
        }
    }
}
