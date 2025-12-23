using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using UnityEngine;

/// <summary>
/// 보상 슬롯 프리팹에 붙여서 카드 이미지 및 획득 수량 텍스트를 갱신해주는 스크립트
/// </summary>

public class RewardSlotUI : MonoBehaviour
{
    [SerializeField] private Image _cardImg;
    [SerializeField] private TextMeshProUGUI _amountText;

    public void InitSlot(int cardId, int amount)
    {
        // 카드 데이터 및 이미지 가져와서 설정해주기
        var cardData = DataManager.Instance.GetCard(cardId);
        if(cardData != null)
        {
            // 카드 이미지갖고오기
        }

        _amountText.text = $"x {amount}";
    }
}
