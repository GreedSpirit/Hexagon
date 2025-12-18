using System;
using UnityEngine;
public enum StatusEffectType
{
    Null,       // 없음
    Fury,       // 공격력 강화
    Vulnerable, // 취약
    Poison,     // 독
    Burn,       // 화상
}
public enum BuffType // 강화 약화 상태이상 효과 분류
{
    Null,
    Buff,           // 대상 강화
    DeBuff,         // 대상 약화
    DoT,            // 턴 종료 시 피해
}
public enum EffectLogic // 강화 약화 상태이상 효과 적용 형태
{
    Null,
    StatMod,        // 능력치 변경
    DmgTaken,       // 받는 피해
    TurnEndDmg,     // 턴 종료 시 피해
}
public enum DecreaseType // 턴 종료 시 감소 형태
{
    Null,
    Turn,           // 스택 1 감소
    Stack,          // 지속 시간 1 감소
}


public class StatusEffectData : CSVLoad, TableKey
{
    public int Id { get; set; }                 // id
    public string Key { get; set; }             // Status 테이블 Key
    public string Name { get; set; }            // 이름
    public string Desc { get; set; }            // 설명
    public BuffType BuffType { get; set; }      // 효과 분류
    public EffectLogic EffectLogic { get; set; }// 효과 형태
    public float ValueFormula { get; set; }     // 효과 계수, 배율
    public DecreaseType DecreaseType { get; set; }// 턴 종료 시 감소 형태
    public int MaxChar { get; set; }            // 캐릭터 최대 수치
    public int MaxMon { get; set; }             // 몬스터 최대 수치
    public string Img { get; set; }             // 효과 아이콘 이미지



    public void LoadFromCsv(string[] values)
    {
        // 누락 수
        int nullCount = 0;

        if (int.TryParse(values[0], out int idValue))
            Id = idValue;
        else
            Id = 0; // 파싱 실패

        Key = values[1];

        if (string.IsNullOrEmpty(values[2]))
        {
            nullCount++;
            Debug.LogError($"{Key} 의 Name이 비어있습니다.");
        }
        else
            Name = values[2];

        if (string.IsNullOrEmpty(values[3]))
        {
            nullCount++;
            Debug.LogError($"{Key} 의 Desc가 비어있습니다.");
        }
        else
            Desc = values[3];

        if (Enum.TryParse(values[4], out BuffType buffType))
            BuffType = buffType;
        else
        {
            nullCount++;
            Debug.LogError($"{Key} 의 BuffType이 비어있습니다.");
            BuffType = BuffType.Null;
        }

        if (Enum.TryParse(values[5], out EffectLogic effectLogic))
            EffectLogic = effectLogic;
        else
        {
            nullCount++;
            Debug.LogError($"{Key} 의 EffectLogic이 비어있습니다.");
            EffectLogic = EffectLogic.Null;
        }

        if (float.TryParse(values[6], out float valueFormula))
            ValueFormula = valueFormula;
        else
            ValueFormula = 0;

        if (Enum.TryParse(values[7], out DecreaseType decreaseType))
            DecreaseType = decreaseType;
        else
        {
            nullCount++;
            Debug.LogError($"{Key} 의 DecreaseType이 비어있습니다.");
            DecreaseType = DecreaseType.Null;
        }


        if (int.TryParse(values[8], out int maxChar))
            MaxChar = maxChar;
        else
        {
            nullCount++;
            Debug.LogError($"{Key} 의 MaxChar이 비어있습니다.");
            MaxChar = 0;
        }

        if (int.TryParse(values[9], out int maxMon))
            MaxMon = maxMon;
        else
        {
            nullCount++;
            Debug.LogError($"{Key} 의 MaxMon이 비어있습니다.");
            MaxMon = 0;
        }

        if (string.IsNullOrEmpty(values[10]))
        {
            nullCount++;
            Debug.LogError($"{Key} 의 Img가 비어있습니다.");
        }
        else
            Img = values[10];

        if(nullCount >= 8)
        {
            Debug.LogError($"{Key} 의 모든 요소가 비어있습니다.");
        }

        if(MaxChar < 0)
        {
            MaxChar = 0;
            Debug.LogError($"{Key} 의 MaxChar가 음수입니다.");
        }

        if (MaxMon < 0)
        {
            MaxMon = 0;
            Debug.LogError($"{Key} 의 MaxMon가 음수입니다.");
        }
    }
}
