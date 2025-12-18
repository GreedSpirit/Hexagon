using UnityEngine;

public class CardLogic : MonoBehaviour
{
    public CardData Data { get; private set; } // 데이터
    private HandManager _handManager;

    public void Init(CardData data, HandManager manager)
    {
        // 카드 데이터
        Data = data;

        _handManager = manager;
    }

    // 카드 사용 시도
    public void TryUse()
    {
        // 계산된 카드 수치 (상태이상 X)
        int cardValue = Data.GetCardValue();

        // 모든 행동 실행
        foreach (ICardAction action in Data.CardActions)
        {
            // 타겟 설정
            IBattleUnit target = Data.Target == Target.Self ? _handManager.TargetPlayer : _handManager.TargetMonster;

            // 사용 (상태이상 키, 카드 계산 수치, 상태이상 부여 수치, 턴 수치, 적용 대상)
            action.Use(Data.StatusEffect, cardValue, Data.StatusEffectValue, Data.Turn, target);
        }

        // 핸드 제거
        _handManager.UseCard(gameObject);
    }
}
