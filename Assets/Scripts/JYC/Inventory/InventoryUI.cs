using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] Transform contentParent;       // Scroll View의 Content
    [SerializeField] GameObject slotPrefab;         // InventorySlotUI 프리팹
    [SerializeField] TMP_Dropdown sortDropdown;     // 정렬 드롭다운
    [SerializeField] TextMeshProUGUI modeText;   //  현재 모드를 표시할 텍스트 (인벤토리 덱편집,전체 보기 전환용)
    private List<InventorySlotUI> _slots = new List<InventorySlotUI>();
    private InventorySlotUI _currentSelectedSlot;   // 현재 선택된 슬롯

    // 현재 덱 편성 모드인지 여부 (외부에서 설정)
    public bool IsDeckBuildingMode = false;

    private const string SORT_SAVE_KEY = "InventorySortType"; // 저장할 때 쓸 이름표

    private void Start()
    {
        // 드롭다운 리스너 연결 (값이 바뀌면 OnSortChanged 실행)
        sortDropdown.onValueChanged.AddListener(OnSortChanged);

        // 저장된 정렬 방식 불러오기 (정렬 방식 유지)
        // 저장된 값이 없으면 0(기본값: 등급순)을 가져옴
        int savedSortIndex = PlayerPrefs.GetInt(SORT_SAVE_KEY, 0);

        // UI와 데이터 매니저에 저장된 값 적용
        sortDropdown.value = savedSortIndex;
        InventoryManager.Instance.CurrentSortType = (InventoryManager.SortType)savedSortIndex;

        // 인벤토리 초기화
        RefreshInventory();
    }

    // 인벤토리 갱신 (데이터 로드 -> 정렬 -> 슬롯 생성)
    public void RefreshInventory()
    {
        // 기존 슬롯 삭제
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);
        _slots.Clear();

        // 매니저에서 정렬된 리스트 가져오기
        var list = InventoryManager.Instance.GetSortedList(IsDeckBuildingMode);

        // 슬롯 생성
        foreach (var userCard in list)
        {
            
            GameObject go = Instantiate(slotPrefab, contentParent);
            var slot = go.GetComponent<InventorySlotUI>();
            slot.Init(userCard, this);
            _slots.Add(slot);
        }
    }

    // 정렬 변경 시 호출되는 함수
    private void OnSortChanged(int index)
    {
        // 매니저의 정렬 타입 변경
        InventoryManager.Instance.CurrentSortType = (InventoryManager.SortType)index;

        // 변경된 정렬 방식 저장
        PlayerPrefs.SetInt(SORT_SAVE_KEY, index);
        PlayerPrefs.Save();

        // 초기화
        RefreshInventory();
    }

    // 자식 슬롯이 클릭되었을 때 호출됨
    public void OnSlotClicked(InventorySlotUI clickedSlot)
    {
        // 이미 선택된 걸 다시 누르면 -> 선택 해제
        if (_currentSelectedSlot == clickedSlot)
        {
            DeselectAll();
            return;
        }

        // 다른 걸 누르면 -> 기존 거 해제하고 새거 선택
        if (_currentSelectedSlot != null)
        {
            _currentSelectedSlot.SetSelected(false);
        }

        clickedSlot.SetSelected(true);
        _currentSelectedSlot = clickedSlot;
    }

    // 배경 클릭 시 선택 해제 (UI 배경 버튼 등에 연결)
    public void DeselectAll()
    {
        if (_currentSelectedSlot != null)
        {
            _currentSelectedSlot.SetSelected(false);
            _currentSelectedSlot = null;
        }
    }
    public void OnClickToggleDeckMode()
    {
        // 모드 반전 (true -> false, false -> true)
        IsDeckBuildingMode = !IsDeckBuildingMode;

        // 화면 갱신 (그래야 필터링이 적용됨)
        RefreshInventory();

        // 텍스트 변경으로 현재 상태 알려주기
        if (modeText != null)
        {
            modeText.text = IsDeckBuildingMode ? "현재: 덱 편집 모드" : "현재: 전체 보기";
        }
    }

}