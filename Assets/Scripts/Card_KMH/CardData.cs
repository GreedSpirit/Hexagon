using UnityEngine;

public enum CardGrade   // 카드 등급
{
    Null,       // 불러오기 실패
    Common,     // 일반, Page
    Rare,       // 희귀, Index
    Epic,       // 영웅, Title
    Legendary,  // 전설, Codex
    Quest,      // 퀘스트, Quest
}
public enum CardType    // 카드 타입
{
    Null,       // 불러오기 실패
    Attack,     // 공격
    Defence,    // 방어
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
    public bool isCard { get; set; }            // 카드 or 스킬
    public string Desc { get; set; }            // 설명
    public CardGrade CardGrade { get; set; }    // 등급
    public CardType CardType { get; set; }      // 타입
    public string CardImg { get; set; }         // 카드 이미지
    public Target Target { get; set; }          // 스킬 적용 대상
    public int Attack { get; set; }             // 피해량
    public int Healing { get; set; }            // 치유량
    public int Shield { get; set; }             // 보호막량
    public string StatusEffect { get; set; }    // StatusEffect 테이블 Key
    public float StatusEffectValue { get; set; }// 스킬 사용 시 적용될 상태이상 스킬 효과
    public int Turn { get; set; }               // 강화, 약화 지속 턴 수
    public string Vfx { get; set; }             // VFX 

    public void LoadFromCsv(string[] values)
    {
        if (int.TryParse(values[0], out int idValue))
            Id = idValue;
        else
            Id = 0; // 파싱 실패

        Key = values[1];

        Name = values[2];

        if (bool.TryParse(values[3], out bool isCard))
            this.isCard = isCard;
        else
            this.isCard = false;

        Desc = values[4];

        if (int.TryParse(values[5], out int grade))
            CardGrade = (CardGrade)grade;
        else
            CardGrade = CardGrade.Null;

        if (int.TryParse(values[6], out int type))
            CardType = (CardType)type;
        else
            CardType = CardType.Null;

        CardImg = values[7];

        if (int.TryParse(values[8], out int target))
            Target = (Target)target;
        else
            Target = Target.Null;

        if (int.TryParse(values[9], out int atk))
            Attack = atk;
        else
            Attack = 0;

        if (int.TryParse(values[10], out int heal))
            Healing = heal;
        else
            Healing = 0;

        if (int.TryParse(values[11], out int shield))
            Shield = shield;
        else
            Shield = 0;

        StatusEffect = values[12];

        if (float.TryParse(values[13], out float statusValue))
            StatusEffectValue = statusValue;
        else
            StatusEffectValue = 0;

        if (int.TryParse(values[14], out int turn))
            Turn = turn;
        else
            Turn = 0;

        Vfx = values[15];
    }
}
