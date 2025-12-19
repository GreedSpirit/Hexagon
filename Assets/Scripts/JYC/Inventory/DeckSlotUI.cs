using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class DeckSlotUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI Components")]
    [SerializeField] Image cardImage;       // 카드 그림
    [SerializeField] Image borderImage;     // 등급 테두리 (필요시)

    private int _cardId;

    // 데이터 세팅
    public void Init(int cardId)
    {
        _cardId = cardId;
        var cardData = DataManager.Instance.GetCard(cardId);

        if (cardData != null)
        {
            // 이미지 로드 (리소스 매니저 구현 전이라 임시 처리)
            // cardImage.sprite = Resources.Load<Sprite>(cardData.CardImg);

            // 여기서 등급별 색상 등 추가 로직 가능
        }
    }

    // 클릭 시: 무조건 덱에서 제거
    public void OnPointerClick(PointerEventData eventData)
    {
        // 덱에서 해당 ID 제거 요청 (매니저가 처리하고 OnDeckChanged 알림)
        InventoryManager.Instance.ToggleDeckEquip(_cardId);
    }

    // 마우스 올리면 1.5배 확대
    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = Vector3.one * 1.5f;
    }

    // 마우스 나가면 원래 크기로
    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = Vector3.one;
    }
}