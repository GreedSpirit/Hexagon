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
    [SerializeField] GameObject selectEffect;    // 선택 시 노란색 빛 (On/Off)
    [SerializeField] TextMeshProUGUI countText; // xN 표시
    [SerializeField] TextMeshProUGUI nameText;  // 카드 이름
    [SerializeField] TextMeshProUGUI levelText; // 카드 레벨
    [SerializeField] TextMeshProUGUI typeText;  // 카드 타입
    [SerializeField] TextMeshProUGUI descText;

    [Header("Equip Mark")]
    [SerializeField] GameObject equipMark; // 장착 표시 (이미지+텍스트)

    private UserCard _userCard;
    public UserCard UserCard => _userCard;
    private InventoryUI _parentUI;
    private bool _isSelected = false;
    private static GameObject _dragGhost;

    private Canvas _myCanvas;
    private void Awake()
    {
        // 프리팹에 붙어있는 Canvas를 찾아옵니다.
        _myCanvas = GetComponent<Canvas>();
    }

    // 데이터 세팅
    public void Init(UserCard userCard, InventoryUI parent)
    {
        _userCard = userCard;
        _parentUI = parent;
        var data = userCard.GetData();

        // UI 갱신
        if (data != null)
        {
            // 일러스트
            if (cardImage != null)
                cardImage.sprite = DataManager.Instance.GetCardSprite(data.CardImg);

            // 텍스트 정보 갱신 (이름)
            if (nameText != null)
            {
                nameText.text = data.Name;
            }

            // 텍스트 정보 갱신 (레벨)
            if (levelText != null)
            {
                levelText.text = userCard.Level.ToString();
            }

            // 텍스트 정보 갱신 (타입 - 한글 변환)
            if (typeText != null)
            {
                string typeString = "";
                switch (data.CardType)
                {
                    case CardType.Attack: typeString = "<color=red>공격</color>"; break;
                    case CardType.Shield: typeString = "<color=blue>방어</color>"; break;
                    case CardType.Healing: typeString = "<color=green>치유</color>"; break;
                    case CardType.Spell: typeString = "<color=purple>주문</color>"; break;
                    default: typeString = "기타"; break;
                }
                typeText.text = typeString;
            }
            if (descText != null)
            {
                descText.text = ParseDescription(data, userCard.Level);
            }
            SetGradeVisual(data.CardGrade);
        }
        if (countText != null)
        {
            countText.text = $"x {userCard.Count}";
        }
        selectEffect.gameObject.SetActive(false); // 처음엔 꺼둠

        // 장착 여부 확인
        bool isEquipped = InventoryManager.Instance.IsCardInDeck(userCard.CardId);

        // 켜고 끄기
        if (equipMark != null)
        {
            equipMark.SetActive(isEquipped);
        }
        if (_myCanvas != null)
        {
            _myCanvas.overrideSorting = false;
            _myCanvas.sortingOrder = 0;
        }

    }

    private void SetGradeVisual(CardGrade grade)
    {
        if (borderImage == null) return;

        // InventoryManager에 있는 등급별 스프라이트를 가져옴
        Sprite borderSprite = InventoryManager.Instance.GetGradeBorderSprite(grade);

        if (borderSprite != null)
        {
            borderImage.sprite = borderSprite;
            borderImage.color = Color.white; // 이미지가 보이게 하얀색
            borderImage.gameObject.SetActive(true);
        }
        else
        {
            // 이미지가 없으면 끄기
            borderImage.gameObject.SetActive(false);
        }
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
        sb.Replace("{Turns}", data.Turn.ToString());

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
            if (_myCanvas != null)
            {
                _myCanvas.overrideSorting = true;
                _myCanvas.sortingOrder = 20;
            }
        }
        else
        {
            // 원래대로 복귀
            transform.localScale = Vector3.one;
            if (_myCanvas != null)
            {
                _myCanvas.overrideSorting = false;
                _myCanvas.sortingOrder = 0;
            }
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

        transform.localScale = Vector3.one * 1.2f;
        selectEffect.gameObject.SetActive(true);
        if (_myCanvas != null)
        {
            _myCanvas.overrideSorting = true;
            _myCanvas.sortingOrder = 20;
        }
    }

    // 마우스 나갈 때 복구
    public void OnPointerExit(PointerEventData eventData)
    {
        if (_isSelected) return; // 선택된 상태면 줄어들지 않음

        transform.localScale = Vector3.one;
        selectEffect.gameObject.SetActive(false);
        if (_myCanvas != null)
        {
            _myCanvas.overrideSorting = false;
            _myCanvas.sortingOrder = 0;
        }
    }
}