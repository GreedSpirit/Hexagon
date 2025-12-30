using UnityEngine;

[CreateAssetMenu(fileName = "newVulnerableAction", menuName = "Card Actions/Vulnerable Action")]
public class CardVulnerableAction : CardActionStatus
{
    public override void Use(string statusEffectKey, int statusValue, int turn, IBattleUnit target)
    {
        base.Use(statusEffectKey, statusValue, turn, target);

        // 스택, 지속시간 둘 다 0 이면 무시
        if (turn == 0 && statusValue == 0) return;

        Debug.Log($"취약 사용 : {turn} 턴 지속");
        target.AddStatusEffect(statusEffectKey, turn, statusValue);
    }
}