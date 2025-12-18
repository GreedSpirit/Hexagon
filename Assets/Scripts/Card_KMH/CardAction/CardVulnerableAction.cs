using UnityEngine;

public class CardVulnerableAction : ICardAction
{
    public void Use(StatusEffectData statusData, int value, int statusValue, int turn, IBattleUnit target)
    {
        if (target == null)
        {
            Debug.LogError("Target 이 Null 입니다.");
            return;
        }
        Debug.Log("취약 사용");
        //target.적용();
    }
}