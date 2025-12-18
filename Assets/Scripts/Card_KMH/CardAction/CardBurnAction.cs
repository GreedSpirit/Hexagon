using UnityEngine;

public class CardBurnAction : ICardAction
{
    public void Use(StatusEffectData statusData, int value, IBattleUnit target)
    {
        if (target == null)
        {
            Debug.LogError("Target 이 Null 입니다.");
            return;
        }
        Debug.Log("화상 상태이상 사용");
        //target.상태이상 적용();
    }
}
