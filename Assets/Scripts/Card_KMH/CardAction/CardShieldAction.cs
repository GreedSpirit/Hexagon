using UnityEngine;

public class CardShieldAction : ICardAction
{
    // 카드 사용
    public void Use(string statusEffectKey, int value, int statusValue, int turn, IBattleUnit target)
    {
        if(target == null)
        {
            Debug.LogError("Target 이 Null 입니다.");
            return;
        }
        Debug.Log($"방어 카드 사용 : 보호막 {value} 생성.");
        target.GetShield(value);
    }
}
