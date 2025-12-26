using UnityEngine;

public class CardAction : ScriptableObject, ICardAction
{
    [Header("사용 오디오 클립")]
    [SerializeField] AudioClip _useClip;

    // 카드 사용
    public virtual void Use(int value, IBattleUnit target)
    {
        if (target == null)
        {
            Debug.LogError("Target 이 Null 입니다.");
            return;
        }

        SoundManager.Instance.PlaySFX(_useClip);
    }

    // 주문 카드 사용
    public virtual void Use(string statusEffectKey, int statusValue, int turn, IBattleUnit target)
    {
        if (target == null)
        {
            Debug.LogError("Target 이 Null 입니다.");
            return;
        }

        SoundManager.Instance.PlaySFX(_useClip);
    }
}
