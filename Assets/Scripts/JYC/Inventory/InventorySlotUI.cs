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

    //
    // 드래그 관련 (화살표 로직은 DragManager가 처리한다고 가정) 
    // 관련 기능에 맞춰서 수정 예정
    // 주석 처리한 나머지 코드도 맞춰서 수정
    //

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!_isSelected)
        {
            _parentUI.OnSlotClicked(this); // 부모에게 선택 요청
        }
        var cardData = _userCard.GetData();
        // [체크] 타겟이 'Self'인 경우 (치유, 방어 등)
        if (cardData.Target == Target.Self) // Target Enum에 Self가 있다고 가정
        {
            Debug.Log("드래그 시작: [Self 타겟] - 화살표 대신 캐릭터 강조 연출 필요");
            // DragManager.Instance.StartDragSelfTarget(this); // 예시: 자신에게 드롭 유도
        }
        else
        {
            Debug.Log("드래그 시작: [Enemy 타겟] - 화살표 생성");
            // DragManager.Instance.StartDragArrow(this, eventData.position); // 예시: 화살표
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!_isSelected) return;

        // DragManager.Instance.UpdateDrag(eventData.position);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // DragManager.Instance.EndDrag();
        Debug.Log("드래그 종료: 덱 슬롯에 닿았는지 확인");
    }
}