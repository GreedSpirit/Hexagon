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
            // 카드 이미지 (나중에 리소스 로드 구현 시 주석 해제)
            // cardImage.sprite = Resources.Load<Sprite>(cardData.CardImg);

            // 등급별 테두리 설정 (색상 or 이미지)
            SetGradeVisual(cardData.CardGrade);
        }
    }
    private void SetGradeVisual(CardGrade grade)
    {
        if (borderImage == null) return;

        // [Option A: 에셋 받기 전 (색깔로 구분)]
        switch (grade)
        {
            case CardGrade.Common: borderImage.color = Color.gray; break;
            case CardGrade.Rare: borderImage.color = Color.cyan; break;
            case CardGrade.Epic: borderImage.color = Color.magenta; break; // 보라색
            case CardGrade.Legendary: borderImage.color = Color.yellow; break;  // 금색
            default: borderImage.color = Color.white; break;
        }

        // [Option B: 에셋 받은 후 (이미지 교체)]
        // 나중에 에셋 받으면 위 switch문을 지우고 아래 주석 해제
        /*
        string spriteName = "Border_" + grade.ToString(); // 예: Border_Rare
        Sprite gradeSprite = Resources.Load<Sprite>("UI/Borders/" + spriteName);
        if (gradeSprite != null) 
        {
            borderImage.sprite = gradeSprite;
            borderImage.color = Color.white; // 색상 틴트 제거
        }
        */
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
    public void OnDrop(PointerEventData eventData)
    {
        // 드래그한 물체가 인벤토리 슬롯인지 확인
        GameObject droppedObj = eventData.pointerDrag;
        if (droppedObj == null) return;

        InventorySlotUI invSlot = droppedObj.GetComponent<InventorySlotUI>();
        if (invSlot != null)
        {
            Debug.Log("인벤토리 카드를 덱 슬롯에 드롭함 -> 교체 시도");
            // 여기에 '교체' 또는 '장착' 로직 호출 예정
        }
    }
}