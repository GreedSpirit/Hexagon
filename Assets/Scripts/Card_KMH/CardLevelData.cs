using UnityEngine;

public enum StatusEffect
{
    None = 1,
}

public class CardLevelData
{
    public int Id { get; set; }                     // id
    public string CardLevelKey { get; set; }        // 카드 레벨 테이블 키
    public string Card { get; set; }                // 카드 테이블 키
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
}
