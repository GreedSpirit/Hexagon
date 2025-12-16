using UnityEngine;

public class CardAttackAction : ICardAction
{
    // 카드 사용
    public void Use(CardData data, int value)
    {
        Debug.Log($"공격 카드 사용 : {value} 피해.");
    }


    // 계산된 값 반환
    public int GetValue(CardData data)
    {
        Debug.Log($"{data.Name} 계산 / {data.BaseValue} + {data.Level} * {data.ValuePerValue}");
        return data.BaseValue + (data.Level * data.ValuePerValue);
    }
}
