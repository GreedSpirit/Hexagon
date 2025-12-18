using UnityEngine;

public class CardPoisonAction : ICardAction
{
    public void Use(string statusEffectKey, int value, int statusValue, int turn, IBattleUnit target)
    {
        if (target == null)
        {
            Debug.LogError("Target 이 Null 입니다.");
            return;
        }
        Debug.Log($"중독 사용 : {statusValue} 부여");
        target.AddStatusEffect(statusEffectKey, statusValue, turn);
    }
}
