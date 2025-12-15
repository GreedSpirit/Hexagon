using UnityEngine;

public enum Grade   // 카드 등급
{
    Page = 1,   // 일반
    Index,      // 희귀
    Title,      // 영웅
    Codex,      // 전설
    Quest,      // 퀘스트
}
public enum Type    // 카드 타입
{
    Attack = 1, // 공격
    Defence,    // 방어
    Spell,      // 주문
}

public class CardData
{
    public int Id { get; set; }                 // id
    public string CardKey { get; set; }         // 카드 테이블 키
    public string Name { get; set; }            // 이름
    public Grade Grade { get; set; }            // 등급
    public Type Type { get; set; }              // 타입
    public string Desc { get; set; }            // 설명
}
