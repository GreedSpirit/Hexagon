using UnityEngine;

public interface ICardAction
{
    public void Use(CardData data, int value, IBattleUnit target);         // 카드 사용
}
