using UnityEngine;

public class CardHealingAction : ICardAction
{
    // 카드 사용
    public void Use(StatusEffectData statusData, int value, IBattleUnit target)
    {
        if (target == null)
        {
            Debug.LogError("Target 이 Null 입니다.");
            return;
        }
        Debug.Log($"치유 카드 사용 : {value} 회복.");
        target.GetHp(value);
    }
}
