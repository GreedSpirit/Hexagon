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
    private CardGrade _requiredGrade;
    // 드래그 효과를 위한 반투명 이미지 변수
    private GameObject _dragGhost;
    private bool _isDropHandled = false;

    // 데이터 세팅
    public void Init(int cardId, int index, CardGrade requiredGrade)
    {
        _cardId = cardId;
        _slotIndex = index;
        _requiredGrade = requiredGrade;
        // 테두리 색상 설정 (빈 슬롯이어도 등급 색은 나와야 함)
        SetGradeVisual(requiredGrade);
        // 카드가 장착된 상태인가?
        if (_cardId != -1)
        {
            var cardData = DataManager.Instance.GetCard(_cardId);

            // 데이터가 있으면 이미지를, 없으면 그냥 흰색 박스라도 띄웁니다.
            cardImage.gameObject.SetActive(true);

            if (cardData != null)
            {
                // cardImage.sprite = Resources.Load<Sprite>(cardData.CardImg); // 나중에 주석 해제
                cardImage.color = Color.white; // 카드 그림이 있으면 흰색(원본색)
            }
            else
            {
                // 데이터 테이블에 없는 카드 ID일 경우 (테스트 중 발생 가능)
                cardImage.color = Color.red; // "에러! 데이터 없음" 표시
            }
        }
        else
        {
            // 빈 슬롯임 -> 카드 이미지를 끄거나 투명하게
            // (배경의 빈 슬롯 모양이 보여야 하므로)
            cardImage.color = Color.clear;
        }
    }

    private void SetGradeVisual(CardGrade grade)
    {
        if (borderImage == null) return;

        switch (grade)
        {
            case CardGrade.Common:
                borderImage.color = Color.black; // 검정
                break;
            case CardGrade.Rare:
                borderImage.color = Color.blue;  // 파랑
                break;
            case CardGrade.Epic:
            case CardGrade.Legendary:
                borderImage.color = new Color(0.5f, 0, 0.5f); // 보라 (Purple)
                break;
            default:
                borderImage.color = Color.white;
                break;
        }
    }
    private void HighlightSlot(bool isOn)
    {
        if (borderImage == null) return;

        if (isOn)
        {
            // 활성화: 파란색으로 빛나게
            borderImage.color = Color.cyan;
            // 만약 나중에 "빛나는 이미지" 에셋을 받는다면, 여기서 그 이미지를 SetActive(true)
        }
        else
        {
            // 비활성화: 원래 등급 색상으로 복구
            SetGradeVisual(_requiredGrade);
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

        // 카드가 있을 때만 해제/교체 로직을 수행하도록 예외 처리합니다.
        if (_cardId == -1) return;

        int selectedInvenCardId = InventoryManager.Instance.GetSelectedCardId();

        if (selectedInvenCardId != -1)
        {
            // 교체 시도 (인덱스가 유효해야 함)
            if (_slotIndex != -1)
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
        // 드래그 중이 아닐 때 -> 그냥 확대 (기존 기능)
        if (!eventData.dragging)
        {
            transform.localScale = Vector3.one * 1.5f;
            return;
        }

        // 드래그 중일 때 -> 유효한 카드면 파란색 테두리
        GameObject draggedObj = eventData.pointerDrag;
        if (draggedObj != null)
        {
            InventorySlotUI invSlot = draggedObj.GetComponent<InventorySlotUI>();
            if (invSlot != null)
            {
                // 등급 조건 체크
                CardData dropData = invSlot.UserCard.GetData();
                bool isMatch = false;
                if (_requiredGrade == CardGrade.Epic)
                    isMatch = (dropData.CardGrade == CardGrade.Epic || dropData.CardGrade == CardGrade.Legendary);
                else
                    isMatch = (dropData.CardGrade == _requiredGrade);

                // 조건이 맞으면 파란색 불 켜기
                if (isMatch)
                {
                    HighlightSlot(true);
                }
            }
        }
    }

    // 마우스 나가면 원래 크기로
    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = Vector3.one;

        // 하이라이트 끄기 (원래 색 복구)
        HighlightSlot(false);
    }

    // 인벤토리에서 카드를 드래그해서 이 슬롯 위에 놓았을 때 (교체)
    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedObj = eventData.pointerDrag;
        if (droppedObj == null) return;

        InventorySlotUI invSlot = droppedObj.GetComponent<InventorySlotUI>();
        if (invSlot != null)
        {
            // 등급 체크
            CardData dropData = invSlot.UserCard.GetData();
            bool isMatch = false;

            if (_requiredGrade == CardGrade.Epic)
                isMatch = (dropData.CardGrade == CardGrade.Epic || dropData.CardGrade == CardGrade.Legendary);
            else
                isMatch = (dropData.CardGrade == _requiredGrade);

            if (!isMatch)
            {
                Debug.Log($"등급이 맞지 않습니다! (필요: {_requiredGrade})");
                return;
            }

            // 드롭 처리 시작
            _isDropHandled = true;
            InventoryManager.Instance.IsDropProcessing = true;

            int newCardId = invSlot.UserCard.CardId;

            //  빈 슬롯이든 아니든, 무조건 ReplaceDeckCardAt 호출
            InventoryManager.Instance.ReplaceDeckCardAt(this._slotIndex, newCardId);
        }
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_cardId == -1) return;
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
        if (_dragGhost != null)
        {
            Destroy(_dragGhost);
            _dragGhost = null;
        }
        if (_cardId == -1) return;
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