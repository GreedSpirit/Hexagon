using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 상태이상 종류 (임의로 작성, 나중에 수정 가능)
public enum StatusEffectType
{
    Null,       // 없음
    Poison,     // 독
    Burn,       // 화상
    Stun,       // 기절
    Freeze,     // 빙결
    Weakness    // 약화
}

[System.Serializable]
public class SkillData : CSVLoad, TableKey
{

    // 엑셀 컬럼명과 일치시킨 프로퍼티
    public int Id { get; set; }
    public string SkillKey { get; set; }        // 엑셀 컬럼명: SkillKey
    public string Name { get; set; }
    public string Desc { get; set; }

    // 수치 데이터
    public int Attack { get; set; }             // 공격력
    public int IncreaseAttack { get; set; }     // 공격력 증가분
    public int Healing { get; set; }            // 치유량
    public int IncreaseHealing { get; set; }    // 치유량 증가분
    public int Shield { get; set; }             // 보호막
    public int IncreaseShield { get; set; }     // 보호막 증가분

    // 상태이상 관련
    public StatusEffectType StatusEffect { get; set; } // 상태이상 종류 (Enum)
    public int StatusEffectStack { get; set; }     // 상태이상 스택
    public int Duration { get; set; }              // 지속 시간


    // 'Key'로 접근할 때 'SkillKey'를 반환하도록 연결
    string TableKey.Key
    {
        get { return SkillKey; }
    }


    // CSV 데이터 파싱
    public void LoadFromCsv(string[] values)
    {
        // 파싱용 임시 변수
        int vInt;

        // 0: Id
        if (int.TryParse(values[0], out vInt)) Id = vInt;
        else Id = 0;

        // 1: SkillKey
        SkillKey = values[1];

        // 2: Name
        Name = values[2];

        // 3: Desc
        Desc = values[3];

        // 4: Attack
        if (int.TryParse(values[4], out vInt)) Attack = vInt;
        else Attack = 0;

        // 5: IncreaseAttack
        if (int.TryParse(values[5], out vInt)) IncreaseAttack = vInt;
        else IncreaseAttack = 0;

        // 6: Healing
        if (int.TryParse(values[6], out vInt)) Healing = vInt;
        else Healing = 0;

        // 7: IncreaseHealing
        if (int.TryParse(values[7], out vInt)) IncreaseHealing = vInt;
        else IncreaseHealing = 0;

        // 8: Shield
        if (int.TryParse(values[8], out vInt)) Shield = vInt;
        else Shield = 0;

        // 9: IncreaseShield
        if (int.TryParse(values[9], out vInt)) IncreaseShield = vInt;
        else IncreaseShield = 0;

        // 10: StatusEffect (Enum 파싱)
        // 엑셀에 정수(0, 1...)로 적혀있다고 가정하고 파싱
        if (int.TryParse(values[10], out vInt))
            StatusEffect = (StatusEffectType)vInt;
        else
            StatusEffect = StatusEffectType.Null;

        // 11: StatusEffectStack
        if (int.TryParse(values[11], out vInt)) StatusEffectStack = vInt;
        else StatusEffectStack = 0;

        // 12: Duration
        if (int.TryParse(values[12], out vInt)) Duration = vInt;
        else Duration = 0;
    }
}