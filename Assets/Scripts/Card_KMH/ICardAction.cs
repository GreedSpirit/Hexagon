using UnityEngine;

public interface ICardAction
{
    public void Use(StatusEffectData statusData, int value, IBattleUnit target);         // 카드 사용
}
