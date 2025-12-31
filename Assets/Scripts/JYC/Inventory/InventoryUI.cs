using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Header("Detail Panel")]
    [SerializeField] CardDetailPanel _detailPanel;

    [Header("UI References")]
    [SerializeField] Transform contentParent;       // Scroll View의 Content
    [SerializeField] GameObject slotPrefab;         // InventorySlotUI 프리팹
    [SerializeField] TMP_Dropdown sortDropdown;     // 정렬 드롭다운
    [SerializeField] TextMeshProUGUI modeText;      //  현재 모드를 표시할 텍스트 (인벤토리 덱편집,전체 보기 전환용)
    [SerializeField] GameObject deckUIGroup;        // 덱 UI 그룹 (덱 슬롯들이 모인 오브젝트)

    [Header("External UI Control")]
    [SerializeField] List<GameObject> _uiToHide;

    [Header("UI Positioning (패널 위치 제어)")]
    [SerializeField] RectTransform inventoryPanelRect; // 인벤토리 패널

    // 배경 이미지 제어용 변수
    [Header("Background Settings")]
    [SerializeField] GameObject _backgroundGroup;
    [SerializeField] Image backgroundPanelImage;   // 배경을 띄워줄 Image 컴포넌트 
    [SerializeField] Sprite deckBuildingBg;        // 덱 편성용 배경 (펼친 책)

    private List<InventorySlotUI> _slots = new List<InventorySlotUI>();
    private InventorySlotUI _currentSelectedSlot;   // 현재 선택된 슬롯

    // 현재 덱 편성 모드인지 여부 (외부에서 설정)
    public bool IsDeckBuildingMode = false;

    private const string SORT_SAVE_KEY = "InventorySortType"; // 저장할 때 쓸 이름표

    private SettingUI _cachedSettingUI;

    private void Start()
    {
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.RegisterInventoryUI(this);
            InventoryManager.Instance.OnDeckChanged += RefreshInventory;
        }
        // 드롭다운 리스너 연결 (값이 바뀌면 OnSortChanged 실행)
        sortDropdown.onValueChanged.AddListener(OnSortChanged);
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnRequestDeselect += DeselectVisualsOnly;
        }
        // 저장된 정렬 방식 불러오기 (정렬 방식 유지)
        // 저장된 값이 없으면 0(기본값: 등급순)을 가져옴
        int savedSortIndex = PlayerPrefs.GetInt(SORT_SAVE_KEY, 0);

        // UI와 데이터 매니저에 저장된 값 적용
        sortDropdown.value = savedSortIndex;
        InventoryManager.Instance.CurrentSortType = (InventoryManager.SortType)savedSortIndex;

        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnDeckChanged += RefreshInventory;
        }
        // 인벤토리 초기화
        RefreshInventory();
        CloseInventory();
    }
    private void OnDestroy()
    {
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnDeckChanged -= RefreshInventory;
            InventoryManager.Instance.OnRequestDeselect -= DeselectVisualsOnly;
        }
    }
    // 인벤토리 갱신 (데이터 로드 -> 정렬 -> 슬롯 생성)
    public void RefreshInventory()
    {
        if (_backgroundGroup != null) _backgroundGroup.SetActive(true);
        if (backgroundPanelImage != null) backgroundPanelImage.gameObject.SetActive(true);

        // 덱 UI 그룹은 덱 편성 모드일 때만 표시
        if (deckUIGroup != null) deckUIGroup.SetActive(IsDeckBuildingMode);

        // 모드와 상관없이 항상 레이아웃 적용
        if (backgroundPanelImage != null && inventoryPanelRect != null)
        {
            RectTransform bgRect = backgroundPanelImage.rectTransform;

            // 책 이미지 & 전체 화면 꽉 채우기
            backgroundPanelImage.sprite = deckBuildingBg;
            backgroundPanelImage.preserveAspect = false;

            bgRect.anchorMin = new Vector2(0f, 0f);
            bgRect.anchorMax = new Vector2(1f, 1f);
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;

            // 리스트 패널: 오른쪽 페이지(0.5 ~ 1.0)에 고정
            inventoryPanelRect.anchorMin = new Vector2(0.5f, 0f);
            inventoryPanelRect.anchorMax = new Vector2(1f, 1f);
            inventoryPanelRect.offsetMin = new Vector2(20, 20);
            inventoryPanelRect.offsetMax = new Vector2(-20, -20);
        }

        // 슬롯 생성 
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);
        _slots.Clear();

        var list = InventoryManager.Instance.GetSortedList(IsDeckBuildingMode);

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
        InventoryManager.Instance.SetSelectedCardId(clickedSlot.UserCard.CardId);
        if (_detailPanel != null)
        {
            _detailPanel.SetCardInfo(clickedSlot.UserCard);
            _detailPanel.transform.SetAsLastSibling();
        }
    }

    // 배경 클릭 시 선택 해제 (UI 배경 버튼 등에 연결)
    public void DeselectAll()
    {
        DeselectVisualsOnly();
        InventoryManager.Instance.SetSelectedCardId(-1);
        if (_detailPanel != null) _detailPanel.Init();
    }
    public void OnClickToggleDeckMode()
    {
        // 모드 반전 (true -> false, false -> true)
        IsDeckBuildingMode = !IsDeckBuildingMode;

        // 화면 갱신
        RefreshInventory();

        // 텍스트 변경으로 현재 상태 알려주기
        if (modeText != null)
        {
            modeText.text = IsDeckBuildingMode ? "현재: 덱 편집 모드" : "현재: 전체 보기";
        }
    }
    // 매니저의 요청으로 비주얼만 끄는 함수
    private void DeselectVisualsOnly()
    {
        if (_currentSelectedSlot != null)
        {
            _currentSelectedSlot.SetSelected(false);
            _currentSelectedSlot = null;
        }

        if (_detailPanel != null)
        {
            _detailPanel.Init();
        }
    }
    // 켜질 때 Player에게 알림 (덱 모드가 아닐 때만)
    private void OnEnable()
    {
        if (_cachedSettingUI == null) _cachedSettingUI = FindFirstObjectByType<SettingUI>();
        if (_cachedSettingUI != null) _cachedSettingUI.enabled = false;

        ControlExternalUI(false); // 외부 UI 끄기
        RefreshInventory();

        if (_detailPanel != null) _detailPanel.Init(); // 상세창 초기화
    }

    // 꺼질 때 Player에게 알림
    private void OnDisable()
    {
        if (_cachedSettingUI == null) _cachedSettingUI = FindFirstObjectByType<SettingUI>();
        if (_cachedSettingUI != null) _cachedSettingUI.enabled = true;

        ControlExternalUI(true); // 외부 UI 켜기
    }
    private void ControlExternalUI(bool isActive)
    {
        if (_uiToHide != null)
        {
            foreach (var ui in _uiToHide)
            {
                if (ui != null) ui.SetActive(isActive);
            }
        }
    }
    //  ESC 키 처리
    private void Update()
    {
        // 덱 편성 모드일 때는 DeckUI가 ESC를 처리하므로 여기선 무시
        if (IsDeckBuildingMode) return;

        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            // 인벤토리 닫기
            CloseInventory();
        }
    }

    public void CloseInventory()
    {
        // 상세 설명창 초기화
        if (_detailPanel != null) _detailPanel.Init();
        InventoryManager.Instance.DeselectAll();

        // UI 끄기
        gameObject.SetActive(false);

        if (_backgroundGroup != null)
        {
            _backgroundGroup.SetActive(false);
        }
        // Player 이동 모드 복구
        if (backgroundPanelImage != null)
        {
            backgroundPanelImage.gameObject.SetActive(false);
        }

        if (Player.Instance != null) Player.Instance.EnterMoveMod();
    }
}