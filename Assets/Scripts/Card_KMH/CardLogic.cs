using UnityEngine;

public class CardLogic : MonoBehaviour
{
    public CardData Data { get; private set; } // 데이터
    private IBattleUnit target;                // 타겟
    private HandManager _handManager;

    public void Init(CardData data, HandManager manager, IBattleUnit newTarget)
    {
        // 카드 데이터
        Data = data;

        _handManager = manager;

        // 타겟
        target = newTarget;
    }

    // 카드 사용 시도
    public void TryUse()
    {
        // 계산된 카드 수치 (상태이상 X)
        int value = Data.GetCardValue();

        // 상태이상 효과 불러오기
        StatusEffectData statusEffect = Data.GetStatusEffectData();

        // 모든 행동 실행
        foreach (ICardAction action in Data.CardActions)
        {
            // 사용 (상태이상 데이터, 계산 수치, 상태이상 부여 수치, 턴 수치, 적용 대상)
            action.Use(statusEffect, value, Data.StatusEffectValue, Data.Turn, target);
        }

        // 핸드 제거
        _handManager.UseCard(gameObject);
    }
}
