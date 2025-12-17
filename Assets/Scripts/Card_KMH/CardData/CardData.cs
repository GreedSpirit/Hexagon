using System;
using UnityEngine;
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
    public float StatusEffectValue { get; set; }// 스킬 사용 시 적용될 상태이상 스킬 효과
    public int Turn { get; set; }               // 강화, 약화 지속 턴 수
    public string CardImg { get; set; }         // 카드 이미지


    public int Level { get; set; }              // 레벨
    public ICardAction CardAction { get; set; } // 카드 행동



    // 카드 레벨
    public void SetCardLevel(int level)
    {
        Level = level;
    }

    // 카드 동작 설정
    public void SetCardAction()
    {
        // 카드 타입에 맞는 동작 가져오기
        CardAction = TestGameManager_KMH.Instance.GetAction(CardType);
    }

    // 카드 수치 계산 반환
    public int GetCardValue()
    {
        return BaseValue + ((Level - 1) * ValuePerValue);
    }

    // 설명에 수치 적용
    public string GetCardDescWithValue()
    {
        return GetCardDesc(Desc, GetCardValue());
    }

    // 문자 교체 {N} -> 수치
    private string GetCardDesc(string desc, int value)
    {
        Debug.Log(desc);
        Debug.Log(value);
        string newDesc;
        newDesc = desc.Replace("{N}", value.ToString());
        return newDesc;
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

        if (float.TryParse(values[11], out float statusValue))
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
