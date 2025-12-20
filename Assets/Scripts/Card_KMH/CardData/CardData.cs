using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;

public enum CardGrade   // 카드 등급
{
    Null,       // 불러오기 실패
    Common,     // 일반, Page
    Rare,       // 희귀, Index
    Epic,       // 영웅, Title
    Legendary,  // 전설, Codex
}
public enum CardType    // 카드 타입
{
    Null,       // 불러오기 실패
    Attack,     // 공격
    Healing,    // 치유
    Shield,     // 방어
    Spell,      // 주문
}
public enum Target
{
    Null,       // 불러오기 실패
    Self,       // 자신
    Enemy,      // 상대방
}   // 스킬 적용 대상

public class CardData : CSVLoad, TableKey
{
    public int Id { get; set; }                 // id
    public string Key { get; set; }             // 카드 테이블 Key
    public string Name { get; set; }            // 이름
    public bool IsCard { get; set; }            // 카드 or 스킬
    public string Desc { get; set; }            // 설명
    public CardGrade CardGrade { get; set; }    // 등급
    public Target Target { get; set; }          // 스킬 적용 대상
    public CardType CardType { get; set; }      // 타입
    public int BaseValue { get; set; }          // 기본 능력치 수치
    public int ValuePerValue { get; set; }      // 강화 증가 능력치 수치
    public string StatusEffect { get; set; }    // StatusEffect 테이블 Key
    public int StatusEffectValue { get; set; }  // 스킬 사용 시 적용될 상태이상 스킬 효과
    public int Turn { get; set; }               // 강화, 약화 지속 턴 수
    public string CardImg { get; set; }         // 카드 이미지


    // 한글 설정
    public void SetString()
    {
        // 이름
        StringData stringNameData = DataManager.Instance.GetString(Name);
        // 설명
        StringData stringDescData = null;


        if (stringNameData != null) Name = stringNameData.Korean;
        else
            Debug.LogError($"Id {Id} 카드의 {Name} 이 String 테이블에 존재하지 않습니다.");

        // 카드
        if (IsCard == true)
        {
            // Desc 비어있는지 체크
            if(string.IsNullOrEmpty(Desc) == false)
            {
                // 가져오기 시도
                stringDescData = DataManager.Instance.GetString(Desc);
                // 있으면 한글 설정
                if (stringDescData != null) Desc = stringDescData.Korean;
                else Debug.LogError($"Id {Id} 카드의 {Desc} 이 String 테이블에 존재하지 않습니다.");
            }
            else
                Debug.LogError($"Id {Id} 카드의 Desc 가 비어있습니다.");
        }
        // 스킬
        else
        {
            Desc = "";
        }
    }

    // 강화, 약화일 때 StatusEffectValue 를 0 으로
    // DoT일 때 Turn 을 0으로
    public void SetStatusValue()
    {
        // 상태이상 비어있으면 무시
        if (string.IsNullOrEmpty(StatusEffect)) return;

        StatusEffectData statusEffect = DataManager.Instance.GetStatusEffectData(StatusEffect);

        // 혹시나 이름 안맞으면
        if (statusEffect == null) return;

        if (statusEffect.BuffType == BuffType.Buff || statusEffect.BuffType == BuffType.DeBuff)
            StatusEffectValue = 0;
        else
            Turn = 0;
    }

    public void LoadFromCsv(string[] values)
    {
        if (int.TryParse(values[0], out int idValue))
            Id = idValue;
        else
            Id = 0; // 파싱 실패

        Key = values[1];

        if (string.IsNullOrEmpty(values[2]))
            Debug.LogError($"{Key} 의 Name이 비어있습니다.");
        else
            Name = values[2];

        if (bool.TryParse(values[3], out bool isCard))
            IsCard = isCard;
        else
            IsCard = false;

        if (Enum.TryParse(values[5], out CardGrade grade))
            CardGrade = grade;
        else
        {
            Debug.LogError($"{Key} 의 CardGrade가 비어있습니다.");
            CardGrade = CardGrade.Null;
        }

        if (Enum.TryParse(values[6], out Target target))
            Target = target;
        else
        {
            Debug.LogError($"{Key} 의 Target이 비어있습니다.");
            Target = Target.Null;
        }

        if (Enum.TryParse(values[7], out CardType type))
            CardType = type;
        else
        {
            Debug.LogError($"{Key} 의 CardType이 비어있습니다.");
            CardType = CardType.Null;
        }

        if (int.TryParse(values[8], out int baseValue))
            BaseValue = baseValue;
        else
            BaseValue = 0;

        if (int.TryParse(values[9], out int valuePerLevel))
            ValuePerValue = valuePerLevel;
        else
            ValuePerValue = 0;

        StatusEffect = values[10];

        if (int.TryParse(values[11], out int statusValue))
            StatusEffectValue = statusValue;
        else
            StatusEffectValue = 0;

        if (int.TryParse(values[12], out int turn))
            Turn = turn;
        else
            Turn = 0;

        // 카드만 설명, 이미지
        if (IsCard == true)
        {
            Desc = values[4];
            CardImg = values[13];
        }

        // 공격 카드인데 타겟이 Self라면 Enemy로
        if (CardType == CardType.Attack && Target == Target.Self)
            Target = Target.Enemy;

        // 치유 카드인데 타겟이 Enemy라면 Self로
        if (CardType == CardType.Healing && Target == Target.Enemy)
            Target = Target.Self;
    }
}
