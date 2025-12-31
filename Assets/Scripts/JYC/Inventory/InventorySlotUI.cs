using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InventorySlotUI : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler,
    IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI Components")]
    [SerializeField] Image cardImage;       // 카드 일러스트
    [SerializeField] Image backgroundImage;
    [SerializeField] Image borderImage;     // 등급 테두리

    [Header("등급 표시 (색상)")]
    [SerializeField] Image gradeColorImg;       // 우측 상단 네모칸
    [SerializeField] Color[] gradeColors;       // 등급별 색상 (0:Common, 1:Rare, 2:Epic, 3:Legendary)

    [SerializeField] GameObject selectEffect;    // 선택 시 노란색 빛 (On/Off)
    [SerializeField] TextMeshProUGUI countText; // xN 표시
    [SerializeField] TextMeshProUGUI nameText;  // 카드 이름
    [SerializeField] TextMeshProUGUI levelText; // 카드 레벨
    [SerializeField] TextMeshProUGUI noaText;   // 카드 사용가능횟수
    [SerializeField] TextMeshProUGUI typeText;  // 카드 타입
    [SerializeField] TextMeshProUGUI descText;

    [Header("Equip Mark")]
    [SerializeField] GameObject equipMark; // 장착 표시 (이미지+텍스트)

    private UserCard _userCard;
    public UserCard UserCard => _userCard;
    private InventoryUI _parentUI;
    private bool _isSelected = false;
    private static GameObject _dragGhost;
    private int _originalIndex;
    private Canvas _myCanvas;
    private LayoutElement _layoutElement;
    private GameObject _dummy;
    private int _savedIndex;
    private CardTooltipLogic _tooltipLogic;
    private bool _canHoverZoom = true;
    private void Awake()
    {
        // 프리팹에 붙어있는 Canvas를 찾아옵니다.
        _myCanvas = GetComponent<Canvas>();
        _layoutElement = GetComponent<LayoutElement>();
        _tooltipLogic = GetComponent<CardTooltipLogic>();
        if (_myCanvas != null) _myCanvas.overrideSorting = false;
    }

    // 데이터 세팅
    public void Init(UserCard userCard, InventoryUI parent)
    {
        _userCard = userCard;
        _parentUI = parent;
        _canHoverZoom = true;
        UpdateSlotVisual();

        if (_tooltipLogic != null && parent.TooltipUI != null)
            _tooltipLogic.Init(userCard, parent.TooltipUI);

    }
    public void Init(UserCard userCard, CardTooltipUI tooltipUI)
    {
        _userCard = userCard;
        _canHoverZoom = false;
        // 카드 비주얼 업데이트 등 필수 로직만 실행
        UpdateSlotVisual();

        // 인벤토리 매니저 없이 직접 받은 툴팁 UI로 초기화
        if (_tooltipLogic != null && _userCard != null && tooltipUI != null)
        {
            _tooltipLogic.Init(_userCard, tooltipUI);
        }
    }
    private void UpdateSlotVisual()
    {
        if (_userCard == null) return;
        var data = _userCard.GetData();
        if (data == null) return;

        // 일러스트 갱신
        if (cardImage != null)
            cardImage.sprite = DataManager.Instance.GetCardSprite(data.CardImg);

        // 이름 갱신
        if (nameText != null) nameText.text = data.Name;

        // 레벨 갱신
        if (levelText != null) levelText.text = _userCard.Level.ToString();

        // 타입 갱신
        if (typeText != null)
        {
            string typeString = "";
            switch (data.CardType)
            {
                case CardType.Attack: typeString = "공격"; break;
                case CardType.Shield: typeString = "방어"; break;
                case CardType.Healing: typeString = "치유"; break;
                case CardType.Spell: typeString = "주문"; break;
                default: typeString = "기타"; break;
            }
            typeText.text = typeString;
        }

        // 설명 갱신
        if (descText != null) descText.text = ParseDescription(data, _userCard.Level);

        // 등급 색상 갱신
        SetGradeColor(data.CardGrade);

        // 사용 가능 횟수 갱신
        int numberOfAvailable = CardManager.Instance.GetCardNumberOfAvailable(_userCard.Level, data.CardGrade);
        if (noaText != null) noaText.text = numberOfAvailable.ToString("N0");

        // 보유 개수 갱신
        if (countText != null) countText.text = $"x {_userCard.Count}";

        // 장착 표시 갱신
        bool isEquipped = InventoryManager.Instance.IsCardInDeck(_userCard.CardId);
        if (equipMark != null) equipMark.SetActive(isEquipped);

        // 캔버스 정렬 초기화
        if (_myCanvas != null)
        {
            _myCanvas.overrideSorting = false;
            _myCanvas.sortingOrder = 0;
        }

        if (selectEffect != null) selectEffect.gameObject.SetActive(false);
    }
    private void SetGradeColor(CardGrade grade)
    {
        if (gradeColorImg == null || gradeColors == null || gradeColors.Length == 0) return;

        Color color = Color.white; // 기본값

        switch (grade)
        {
            case CardGrade.Common:
                if (gradeColors.Length > 0) color = gradeColors[0];
                break;
            case CardGrade.Rare:
                if (gradeColors.Length > 1) color = gradeColors[1];
                break;
            case CardGrade.Epic:
                if (gradeColors.Length > 2) color = gradeColors[2];
                break;
            case CardGrade.Legendary:
                if (gradeColors.Length > 3) color = gradeColors[3];
                break;
        }

        gradeColorImg.color = color;
        gradeColorImg.gameObject.SetActive(true);
    }
    private string ParseDescription(CardData data, int level)
    {
        if (string.IsNullOrEmpty(data.Desc)) return "";

        // 기본값 + (레벨-1 * 증가량)
        int finalValue = data.BaseValue + (level - 1) * data.ValuePerValue;

        System.Text.StringBuilder sb = new System.Text.StringBuilder(data.Desc);
        sb.Replace("{D}", finalValue.ToString());
        sb.Replace("{N}", finalValue.ToString());
        sb.Replace("{SEV}", data.StatusEffectValue.ToString());
        sb.Replace("{turns}", data.Turn.ToString());

        return sb.ToString();
    }
    // 클릭 이벤트 (확대 및 선택)
    public void OnPointerClick(PointerEventData eventData)
    {

        if (eventData.dragging) return; // 드래그 중이었다면 클릭 무시

        // 부모 UI가 덱 편집 모드인지 확인
        if (_parentUI.IsDeckBuildingMode)
        {
            // 장착/해제 요청
            bool changed = InventoryManager.Instance.ToggleDeckEquip(_userCard.CardId);

            InventoryManager.Instance.DeselectAll();

            if (changed)
            {
                _parentUI.RefreshInventory();
            }
        }
        else
        {
            _parentUI.OnSlotClicked(this);
        }
    }

    // 선택 상태 설정
    public void SetSelected(bool selected)
    {
        _isSelected = selected;
        selectEffect.gameObject.SetActive(selected);

        if (selected)
        {
            // 확대 (기획: 테두리 빛남 + 확대)
            transform.localScale = Vector3.one * 1.2f;
            if (_dummy == null)
            {
                _savedIndex = transform.GetSiblingIndex();

                _dummy = new GameObject("DummySlot", typeof(RectTransform), typeof(LayoutElement));
                _dummy.transform.SetParent(transform.parent);
                _dummy.transform.SetSiblingIndex(_savedIndex);

                RectTransform dummyRT = _dummy.GetComponent<RectTransform>();
                RectTransform myRT = GetComponent<RectTransform>();
                dummyRT.sizeDelta = myRT.sizeDelta;

                if (_layoutElement != null) _layoutElement.ignoreLayout = true;
                transform.SetAsLastSibling();
            }

            _originalIndex = transform.GetSiblingIndex();


            if (_myCanvas != null) _myCanvas.overrideSorting = false;
        }
        else
        {
            if (_dummy != null)
            {
                Destroy(_dummy);
                _dummy = null;
            }
            // 원래대로 복귀
            transform.localScale = Vector3.one;
            if (_layoutElement != null) _layoutElement.ignoreLayout = false;
            transform.SetSiblingIndex(_savedIndex);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 덱 편성 모드가 아니면 드래그 시작 안 함
        if (_parentUI.IsDeckBuildingMode == false) return;

        InventoryManager.Instance.IsDropProcessing = false;

        InventoryManager.Instance.DeselectAll();

        // 드래그용 반투명 아이콘 생성
        _dragGhost = new GameObject("DragGhost");
        _dragGhost.transform.SetParent(_parentUI.transform.root); // Canvas 최상단으로 이동
        _dragGhost.transform.SetAsLastSibling(); // 맨 앞으로 가져오기

        // 이미지 컴포넌트 복사
        Image ghostImg = _dragGhost.AddComponent<Image>();
        ghostImg.sprite = cardImage.sprite;
        ghostImg.color = new Color(1, 1, 1, 1.0f); // 반투명하게
        ghostImg.raycastTarget = false; // 마우스 클릭 통과하게 설정

        // 크기 맞춤
        RectTransform rect = _dragGhost.GetComponent<RectTransform>();
        rect.sizeDelta = GetComponent<RectTransform>().sizeDelta;

        // 초기 위치 설정
        _dragGhost.transform.position = eventData.position;

    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_dragGhost != null)
        {
            _dragGhost.transform.position = eventData.position;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_dragGhost != null) Destroy(_dragGhost);

        if (InventoryManager.Instance.IsDropProcessing)
        {
            // 처리가 끝났으니 플래그 다시 끄고 종료
            InventoryManager.Instance.IsDropProcessing = false;
            Debug.Log("OnEndDrag: 덱 슬롯에서 처리됨 -> 무시");
            return;
        }
    }
    private void OnDisable()
    {
        transform.localScale = Vector3.one; // 크기 초기화

        if (_dragGhost != null) // 고스트 청소
        {
            Destroy(_dragGhost);
            _dragGhost = null;
        }
    }
    // 마우스 오버 시 확대
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_userCard == null) return;
        if (eventData.dragging) return;
        if (_isSelected) return; // 이미 선택(클릭)된 상태라면 크기 유지

        if (_canHoverZoom)
        {
            _savedIndex = transform.GetSiblingIndex();
            _dummy = new GameObject("DummySlot", typeof(RectTransform), typeof(LayoutElement));
            _dummy.transform.SetParent(transform.parent);
            _dummy.transform.SetSiblingIndex(_savedIndex);

            RectTransform dummyRT = _dummy.GetComponent<RectTransform>();
            dummyRT.sizeDelta = GetComponent<RectTransform>().sizeDelta;

            transform.localScale = Vector3.one * 1.2f;
            if (_layoutElement != null) _layoutElement.ignoreLayout = true;
            transform.SetAsLastSibling();
        }

        // 툴팁은 확대 여부와 상관없이
        if (_tooltipLogic != null) _tooltipLogic.PointerEnterParent();

        selectEffect.gameObject.SetActive(true);
    }

    // 마우스 나갈 때 복구
    public void OnPointerExit(PointerEventData eventData)
    {
        if (_isSelected) return; // 선택된 상태면 줄어들지 않음
        if (_tooltipLogic != null) _tooltipLogic.PointerExitParent();
        if (_canHoverZoom)
        {
            if (_dummy != null) Destroy(_dummy);
            transform.localScale = Vector3.one;
            if (_layoutElement != null) _layoutElement.ignoreLayout = false;
            transform.SetSiblingIndex(_savedIndex);
        }
        selectEffect.gameObject.SetActive(false);
    }

}