using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class DeckSlotUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IDropHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("UI Components")]
    [SerializeField] Image cardImage;       // 카드 그림
    [SerializeField] Image borderImage;     // 등급 테두리 (필요시)

    private int _cardId;
    private int _slotIndex;
    // 드래그 효과를 위한 반투명 이미지 변수
    private static GameObject _dragGhost;
    private bool _isDropHandled = false;

    // 데이터 세팅
    public void Init(int cardId, int index)
    {
        _cardId = cardId;
        _slotIndex = index;
        var cardData = DataManager.Instance.GetCard(cardId);

        if (cardData != null)
        {
            // 나중에 리소스 로드 구현 시 주석 해제
            // cardImage.sprite = Resources.Load<Sprite>(cardData.CardImg);

            // 등급별 테두리 설정
            SetGradeVisual(cardData.CardGrade);
        }
    }

    private void SetGradeVisual(CardGrade grade)
    {
        if (borderImage == null) return;

        switch (grade)
        {
            case CardGrade.Common: borderImage.color = Color.gray; break;
            case CardGrade.Rare: borderImage.color = Color.cyan; break;
            case CardGrade.Epic: borderImage.color = Color.magenta; break;
            case CardGrade.Legendary: borderImage.color = Color.yellow; break;
            default: borderImage.color = Color.white; break;
        }
    }

    // 클릭 시: 교체 혹은 제거
    public void OnPointerClick(PointerEventData eventData)
    {
        // 드래그 중이거나, 방금 드롭 처리를 했다면 클릭 무시
        if (eventData.dragging || _isDropHandled)
        {
            _isDropHandled = false; // 다음을 위해 플래그 초기화
            return;
        }

        int selectedInvenCardId = InventoryManager.Instance.GetSelectedCardId();

        if (selectedInvenCardId != -1)
        {
            InventoryManager.Instance.ReplaceDeckCardAt(this._slotIndex, selectedInvenCardId);
            InventoryManager.Instance.DeselectAll();
        }
        else
        {
            InventoryManager.Instance.ToggleDeckEquip(_cardId);
        }
    }

    // 마우스 올리면 확대
    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = Vector3.one * 1.5f;
    }

    // 마우스 나가면 원래 크기로
    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = Vector3.one;
    }

    // 인벤토리에서 카드를 드래그해서 이 슬롯 위에 놓았을 때 (교체)
    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedObj = eventData.pointerDrag;
        if (droppedObj == null) return;

        InventorySlotUI invSlot = droppedObj.GetComponent<InventorySlotUI>();
        if (invSlot != null)
        {
            _isDropHandled = true; // 클릭 방지용

            InventoryManager.Instance.IsDropProcessing = true;

            // 교체 요청
            InventoryManager.Instance.ReplaceDeckCardAt(this._slotIndex, invSlot.UserCard.CardId);
        }
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        // 드래그 시작 시 "유령(Ghost)" 이미지 생성
        _dragGhost = new GameObject("DeckDragGhost");
        _dragGhost.transform.SetParent(transform.root); 
        _dragGhost.transform.SetAsLastSibling();

        // 마우스 이벤트 통과시키기 (그래야 놓았을 때 뒤에 있는 UI 감지 가능)
        CanvasGroup group = _dragGhost.AddComponent<CanvasGroup>();
        group.blocksRaycasts = false;

        // 이미지 복사
        Image ghostImg = _dragGhost.AddComponent<Image>();
        ghostImg.sprite = cardImage.sprite;
        ghostImg.color = new Color(1, 1, 1, 0.6f); // 반투명

        // 크기 맞춤
        RectTransform rect = _dragGhost.GetComponent<RectTransform>();
        rect.sizeDelta = GetComponent<RectTransform>().sizeDelta;
        _dragGhost.transform.position = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 마우스 따라다니기
        if (_dragGhost != null)
            _dragGhost.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 드래그 끝: dragGhost 삭제
        if (_dragGhost != null) Destroy(_dragGhost);

        // 마우스를 놓은 위치 확인
        GameObject hitObject = eventData.pointerEnter;

        // 허공에 놓았음 -> 덱에서 제거
        if (hitObject == null)
        {
            InventoryManager.Instance.ToggleDeckEquip(_cardId);
            return;
        }

        // 다른 UI 위에 놓았는데, 그게 덱 슬롯이 아님 (예: 인벤토리 창, 배경 등) -> 제거
        if (hitObject.GetComponent<DeckSlotUI>() == null)
        {
            InventoryManager.Instance.ToggleDeckEquip(_cardId);
        }

        // (만약 다른 덱 슬롯 위에 놓았다면, 아무 일도 안 함 = 취소됨)
    }
    private void OnDisable()
    {
        if (_dragGhost != null)
        {
            Destroy(_dragGhost);
            _dragGhost = null;
        }
    }
}