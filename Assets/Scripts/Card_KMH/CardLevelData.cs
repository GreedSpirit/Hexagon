using System;
using UnityEngine;

public enum StatusEffect
{
    None,
    Rage,       // 분노
}

public class CardLevelData : CSVLoad, TableKey
{
    public int Id { get; set; }                     // id
    public string CardLevelKey { get; set; }        // 카드 레벨 테이블 키
    public string Key { get; set; }                 // 카드 테이블 키
    public int Level { get; set; }                  // 레벨
    public int NumberOfAvailable { get; set; }      // 사용 가능 횟수
    public int Attack { get; set; }                 // 공격력
    public int IncreaseAttack { get; set; }         // 공격력 증가
    public int Healing { get; set; }                // 치유량
    public int IncreaseHealing { get; set; }        // 치유량 증가
    public int Shield { get; set; }                 // 보호막
    public int IncreaseShield { get; set; }         // 보호막 증가
    public StatusEffect StatusEffect { get; set; }  // 상태이상 종류
    public int StatusEffectStack { get; set; }      // 상태이상 대미지
    public int Duration { get; set; }               // 상태이상 지속 시간

    public void LoadFromCsv(string[] values)
    {
        if (int.TryParse(values[0], out int idValue))
            Id = idValue;
        else
            Id = 0;

        CardLevelKey = values[1];

        Key = values[2];

        if (int.TryParse(values[3], out int level))
            Level = level;
        else
            Level = 0;

        if (int.TryParse(values[4], out int numberOfAvailable))
            NumberOfAvailable = numberOfAvailable;
        else
            NumberOfAvailable = 0;

        if (int.TryParse(values[5], out int atk))
            Attack = atk;
        else
            Attack = 0;

        if (int.TryParse(values[6], out int increaseAtk))
            IncreaseAttack = increaseAtk;
        else
            IncreaseAttack = 0;

        if (int.TryParse(values[7], out int heal))
            Healing = heal;
        else
            Healing = 0;

        if (int.TryParse(values[8], out int increaseHeal))
            IncreaseHealing = increaseHeal;
        else
            IncreaseHealing = 0;

        if (int.TryParse(values[9], out int shield))
            Shield = shield;
        else
            Shield = 0;

        if (int.TryParse(values[10], out int increaseShield))
            IncreaseShield = increaseShield;
        else
            IncreaseShield = 0;


        if (Enum.TryParse(values[11], out StatusEffect status))
            StatusEffect = status;
        else
            StatusEffect = StatusEffect.None;


        if (int.TryParse(values[12], out int stack))
            StatusEffectStack = stack;
        else
            StatusEffectStack = 0;


        if (int.TryParse(values[13], out int duration))
            Duration = duration;
        else
            Duration = 0;
    }
}
