using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InventorySlotUI : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("UI Components")]
    [SerializeField] Image cardImage;       // 카드 일러스트
    [SerializeField] Image borderImage;     // 등급 테두리
    [SerializeField] GameObject selectEffect;    // 선택 시 노란색 빛 (On/Off)
    [SerializeField] TextMeshProUGUI countText; // xN 표시

    [Header("Equip Mark")]
    [SerializeField] GameObject equipMark; // 장착 표시 (이미지+텍스트)

    private UserCard _userCard;
    private InventoryUI _parentUI;
    private bool _isSelected = false;
    private static GameObject _dragGhost;

    // 데이터 세팅
    public void Init(UserCard userCard, InventoryUI parent)
    {
        _userCard = userCard;
        _parentUI = parent;
        var data = userCard.GetData();

        // UI 갱신
        // cardImage.sprite = Resources.Load<Sprite>(data.Img); // 이미지 로드 예시 // 이후 에셋 받고 수정
        countText.text = $"x{userCard.Count}";
        selectEffect.gameObject.SetActive(false); // 처음엔 꺼둠

        // 등급별 테두리 색상 변경 로직 추가 예정 // 에셋 자체로 테두리가 다를지 아니면 코드로 테두리만 색상 변경할지 아직 몰라서 보류함

        // 장착 여부 확인
        bool isEquipped = InventoryManager.Instance.IsCardInDeck(userCard.CardId);

        // 켜고 끄기
        if (equipMark != null)
        {
            equipMark.SetActive(isEquipped);
        }

    }

    // 클릭 이벤트 (확대 및 선택)
    public void OnPointerClick(PointerEventData eventData)
    {

        if (eventData.dragging) return; // 드래그 중이었다면 클릭 무시

        // 부모 UI가 덱 편집 모드인지 확인
        if (_parentUI.IsDeckBuildingMode)
        {
            // 매니저에게 "이 카드 덱에 넣어줘/빼줘" 요청
            bool changed = InventoryManager.Instance.ToggleDeckEquip(_userCard.CardId);
            // 변경사항이 있었다면 화면 갱신 (여기서 Init이 다시 불리며 E마크가 켜짐)
            if (changed)
            {
                _parentUI.RefreshInventory();
                return; 
            }
        }
        // 부모 UI에 선택 요청을 보내, 다른 슬롯의 선택을 해제하고 현재 슬롯만 활성화
        _parentUI.OnSlotClicked(this);
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
        }
        else
        {
            // 원래대로 복귀
            transform.localScale = Vector3.one;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 덱 편성 모드가 아니면 드래그 시작 안 함
        if (_parentUI.IsDeckBuildingMode == false) return;

        if (!_isSelected)
        {
            _parentUI.OnSlotClicked(this); // 드래그 시작하면 선택도 같이 되게
        }

        // 드래그용 반투명 아이콘 생성
        _dragGhost = new GameObject("DragGhost");
        _dragGhost.transform.SetParent(_parentUI.transform.root); // Canvas 최상단으로 이동
        _dragGhost.transform.SetAsLastSibling(); // 맨 앞으로 가져오기

        // 이미지 컴포넌트 복사
        Image ghostImg = _dragGhost.AddComponent<Image>();
        ghostImg.sprite = cardImage.sprite;
        ghostImg.color = new Color(1, 1, 1, 0.6f); // 반투명하게 (투명도 60%)
        ghostImg.raycastTarget = false; // 마우스 클릭 통과하게 설정

        // 크기 맞춤
        RectTransform rect = _dragGhost.GetComponent<RectTransform>();
        rect.sizeDelta = GetComponent<RectTransform>().sizeDelta;

        // 초기 위치 설정
        _dragGhost.transform.position = eventData.position;

        Debug.Log("드래그 시작: 생성됨");
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
        // 드래그 끝났을 때 Ghost 삭제
        if (_dragGhost != null)
        {
            Destroy(_dragGhost);
        }

        // 드래그 종료 시 덱 장착/해제 시도 (간이 구현)
        // 덱 편성 모드라면 드래그를 놓았을 때 장착 시도 (요구사항 충족용)
        if (_parentUI.IsDeckBuildingMode)
        {
            bool changed = InventoryManager.Instance.ToggleDeckEquip(_userCard.CardId);
            if (changed)
            {
                _parentUI.RefreshInventory();
            }
        }

        Debug.Log("드래그 종료: 덱에 장착/해제 시도함");
    }
}