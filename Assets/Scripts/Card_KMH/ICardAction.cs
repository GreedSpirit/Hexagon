using UnityEngine;

public interface ICardAction
{
    public void Use(CardData data, int value);         // 카드 사용
    public int GetValue(CardData data);     // 계산된 값 반환
}
