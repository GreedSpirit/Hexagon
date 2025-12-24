using UnityEngine;

public class CardKnowledgeAction : ICardAction
{
    public void Use(string statusEffectKey, int value, int statusValue, int turn, IBattleUnit target)
    {
        if (target == null)
        {
            Debug.LogError("Target 이 Null 입니다.");
            return;
        }

        // 스택, 지속시간 둘 다 0 이면 무시
        if (turn == 0 && statusValue == 0) return;

        Debug.Log($"지식 사용 : {turn} 턴 지속");
        target.AddStatusEffect(statusEffectKey, turn, statusValue);
    }
}
