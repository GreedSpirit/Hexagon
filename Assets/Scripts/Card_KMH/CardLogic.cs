using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class CardLogic : MonoBehaviour
{
    public CardData Data { get; private set; } // 데이터
    private HandManager _handManager;

    public int Level { get; private set; }              // 레벨
    public string Desc { get; private set; }            // 최종 설명
    public int Deal { get; private set; }               // 최종 피해량
    public List<ICardAction> CardActions { get; private set; } // 카드 행동 리스트 (공격, 방어, 치유, 주문, 상태이상)

    public void Init(CardData data, HandManager manager,int level)
    {
        // 카드 데이터
        Data = data;

        // 핸드 매니저
        _handManager = manager;

        // 레벨
        Level = level;

        // 카드 행동
        SetAction();

        // 최종 피해량 갱신
        UpdateDeal(0, 0, 0);

        // 카드 설명
        UpdateDesc();
    }

    // 카드 동작 설정
    private void SetAction()
    {
        CardActions = new List<ICardAction>();

        // 카드 타입에 맞는 동작 가져오기
        if (CardManager.Instance.GetAction(Data.CardType, out ICardAction typeAction))
        {
            CardActions.Add(typeAction);
        }

        // 상태이상, 효과 아니면 무시
        if (string.IsNullOrEmpty(Data.StatusEffect)) return;

        //  카드 상태이상에 맞는 동작 가져오기
        if (CardManager.Instance.GetAction(Data.StatusEffect, out ICardAction statusAction))
        {
            CardActions.Add(statusAction);
        }
    }

    // 최종 피해량 설정
    public void UpdateDeal(float playerBuff, float monsterDebuff, int monsterDef)
    {
        // 공격 타입 일 때
        if (Data.CardType == CardType.Attack)
        {
            Deal = (int)(((Data.BaseValue + (Level - 1) * Data.ValuePerValue) * (1 + playerBuff) - monsterDef) * (1 + monsterDebuff));
        }
    }

    // 설명 갱신
    public void UpdateDesc()
    {
        // 카드 설명 있을 때만
        if (string.IsNullOrEmpty(Data.Desc)) return;

        // 문자열 갱신
        StringBuilder sb = new StringBuilder(Data.Desc);

        sb.Replace("{D}", Deal.ToString());
        sb.Replace("{N}", GetValue().ToString());
        sb.Replace("{SEV}", Data.StatusEffectValue.ToString());
        sb.Replace("{Turns}", Data.Turn.ToString());

        Desc = sb.ToString();
    }

    // 카드 수치 반환
    private int GetValue()
    {
        if (Data.CardType == CardType.Attack) return Deal;
        return Data.BaseValue + (Level - 1) * Data.ValuePerValue;
    }

    // 카드 사용 시도
    public void TryUse()
    {
        // 카드 수치 가져오기
        int cardValue = GetValue();

        // 모든 행동 실행
        foreach (ICardAction action in CardActions)
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
