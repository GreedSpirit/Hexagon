using UnityEngine;

public class CardAttackAction : ICardAction
{
    // 카드 사용
    public void Use(CardData data, int value, IBattleUnit target)
    {
        if (target == null)
        {
            Debug.LogError("Target 이 Null 입니다.");
            return;
        }
        Debug.Log($"공격 카드 사용 : {value} 피해.");
        target.TakeDamage(value);
    }
}
