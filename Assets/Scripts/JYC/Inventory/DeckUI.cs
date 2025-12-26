using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using UnityEngine.SceneManagement;

public class DeckUI : MonoBehaviour
{

    [SerializeField] Transform contentParent;   // Scroll View의 Content
    [SerializeField] GameObject deckSlotPrefab; // DeckSlotUI가 붙은 프리팹
    [SerializeField] TextMeshProUGUI countText;

    [Header("Background Object")]
    [SerializeField] GameObject uiBackground;
    // 인벤토리를 제어하기 위해 연결할 변수
    [Header("External Connection")]
    [SerializeField] InventoryUI _inventoryUI;

    private DungeonData _targetDungeon;

    [Header("Confirmation Popup")]
    [SerializeField] GameObject _enterPopupPanel; // 팝업 패널 전체
    [SerializeField] TextMeshProUGUI _popupText;  // 팝업 내용 텍스트

    [Header("Dungeon Info UI")]
    [SerializeField] TextMeshProUGUI stageInfoText; // "총 N 스테이지" 표시용
    [SerializeField] Button dungeonInfoButton;      // "던전 정보" 팝업 여는 버튼

    private void Start()
    {

        // 매니저의 이벤트 구독
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnDeckChanged += RefreshDeck;

            // 데이터가 있다면 갱신 (내부 데이터 로딩용)
            if (_targetDungeon != null) RefreshDeck();
        }

        // 시작할 때 팝업 꺼두기
        if (_enterPopupPanel != null) _enterPopupPanel.SetActive(false);

    }

    private void OnDestroy()
    {
        // 구독 해제 
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnDeckChanged -= RefreshDeck;
        }
    }
    // DungeonPresenter가 호출해줄 함수
    public void ReadyForBattle(DungeonData dungeon)
    {
        _targetDungeon = dungeon;

        // 인벤토리 매니저에게 이번 던전의 덱 구성 정보(슬롯 개수 등)를 전달해서 세팅하게 함
        DeckData requiredDeck = DataManager.Instance.GetDeck(_targetDungeon.Deck);
        if (requiredDeck != null && InventoryManager.Instance != null)
        {
            InventoryManager.Instance.ConfigureDeckSlots(requiredDeck);
        }

        // 던전 정보 UI 갱신 (총 스테이지 수 표시)
        if (stageInfoText != null && _targetDungeon != null)
        {
            stageInfoText.text = $"총 {_targetDungeon.NumberOfStages} 스테이지";
        }

        // 던전 정보 버튼 연결
        if (dungeonInfoButton != null)
        {
            dungeonInfoButton.onClick.RemoveAllListeners();
            dungeonInfoButton.onClick.AddListener(OnClickDungeonInfo);
        }
        // DeckUI를 켭니다.
        this.gameObject.SetActive(true);
        // 백그라운드도 같이 켜기
        if (uiBackground != null) uiBackground.SetActive(true);
        // UI가 다른 창에 가려지지 않도록 맨 앞으로 가져옵니다.
        this.transform.SetAsLastSibling();

        //  연결된 인벤토리 UI도 강제로 켜고, '덱 편성 모드'로 바꿉니다.
        if (_inventoryUI != null)
        {
            _inventoryUI.gameObject.SetActive(true); // 인벤토리가 꺼져있을 수 있으니 켬
            _inventoryUI.IsDeckBuildingMode = true;  // 모드 변경 (클릭 시 장착/해제)
            _inventoryUI.RefreshInventory();         // 화면 갱신 (모드 변경 반영)
        }

        RefreshDeck();
    }
    private void OnClickDungeonInfo()
    {
        Debug.Log(" 던전 정보 팝업 오픈 (추후 구현 예정)");
        // TODO: 기획서 8.1 팝업과 연결
    }
    // '뒤로가기'나 '닫기' 버튼에 연결할 함수 (마을로 돌아갈 때)
    public void CloseDeckUI()
    {
        GameSaveManager.Instance.LoadGame();
        // DeckUI 끄기
        this.gameObject.SetActive(false);
        if (uiBackground != null) uiBackground.SetActive(false);
        // 인벤토리 정리 (끄거나 모드 해제)
        if (_inventoryUI != null)
        {
            _inventoryUI.IsDeckBuildingMode = false; // 일반 모드로 복구
            _inventoryUI.RefreshInventory();

            // 기획상 마을 화면에서 인벤토리가 꺼져야 한다면 아래 코드 사용
            _inventoryUI.gameObject.SetActive(false);
        }
    }
    // [전투 시작] 버튼에 연결할 최종 함수
    public void OnClickStartBattle()
    {
        // 덱 유효성 검사
        // 기획: 던전마다 요구 카드 수가 다를 수 있음
        if (_targetDungeon == null) return;

        // DeckData에서 등급별 요구 개수 가져오기
        DeckData deckRequirement = DataManager.Instance.GetDeck(_targetDungeon.Deck);

        // 등급별 검증
        if (!ValidateDeckByGrade(deckRequirement))
        {
            Debug.Log("등급별 카드 개수가 맞지 않습니다!");
            return;
        }
        if (_popupText != null)
        {
            string dName = _targetDungeon != null ? _targetDungeon.Name : "Unknown Dungeon";
            _popupText.text = $"선택한 덱으로 <color=yellow>{dName}</color>에\n입장하시겠습니까?";
        }
        // 팝업 활성화
        if (_enterPopupPanel != null) _enterPopupPanel.SetActive(true);

    }
    // 팝업 '예' 클릭 시 -> 진짜 입장
    public void OnClickPopupYes()
    {
        // 최종 저장 (전투 진입 전 상태 저장)
        GameSaveManager.Instance.SaveGame();

        // 진짜 씬 이동
        string dungeonName = _targetDungeon != null ? _targetDungeon.Name : "Unknown Dungeon";
        Debug.Log($"던전 '{dungeonName}'으로 출발합니다!");

        Player.Instance.EnterBattle();

        // 실제 전투 씬 이름으로 로드
        SceneManager.LoadScene("DungeonBattleScene");
    }
    // [취소] 팝업 '아니오' 클릭 시
    private bool ValidateDeckByGrade(DeckData requirement)
    {
        var deck = CardManager.Instance.CurrentDeck;

        // 등급별로 카운트
        int commonCount = 0, rareCount = 0, epicCount = 0;

        foreach (int cardId in deck)
        {
            CardData data = DataManager.Instance.GetCard(cardId);
            switch (data.CardGrade)
            {
                case CardGrade.Common: commonCount++; break;
                case CardGrade.Rare: rareCount++; break;
                case CardGrade.Epic:
                case CardGrade.Legendary: // 기획 : Epic과 Legendary는 같은 등급으로 취급
                    epicCount++;
                    break;
            }
        }

        // 검증
        return commonCount == requirement.NormalCount
            && rareCount == requirement.RareCount
            && epicCount == requirement.EpicCount;
    }
    public void OnClickPopupNo()
    {
        if (_enterPopupPanel != null) _enterPopupPanel.SetActive(false);
    }
    // 덱 리스트 새로고침
    public void RefreshDeck()
    {
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        if (_targetDungeon == null) return;

        DeckData requiredDeck = DataManager.Instance.GetDeck(_targetDungeon.Deck);
        if (requiredDeck == null) return;

        int globalSlotIndex = 0;

        // globalSlotIndex 넘겨서 1:1 매칭
        CreateSlotsForGrade(CardGrade.Common, requiredDeck.NormalCount, ref globalSlotIndex);
        CreateSlotsForGrade(CardGrade.Rare, requiredDeck.RareCount, ref globalSlotIndex);
        CreateSlotsForGrade(CardGrade.Epic, requiredDeck.EpicCount, ref globalSlotIndex);

        if (countText != null)
        {
            int totalRequired = requiredDeck.NormalCount + requiredDeck.RareCount + requiredDeck.EpicCount;
            int currentCount = InventoryManager.Instance.CurrentDeck.Count(id => id != -1);
            countText.text = $"Deck : {currentCount} / {totalRequired}";
        }
    }
    // 슬롯 생성 (인덱스 접근)
    private void CreateSlotsForGrade(CardGrade targetGrade, int count, ref int globalIndex)
    {
        var currentDeck = InventoryManager.Instance.CurrentDeck;

        for (int i = 0; i < count; i++)
        {
            GameObject go = Instantiate(deckSlotPrefab, contentParent);
            DeckSlotUI slot = go.GetComponent<DeckSlotUI>();

            int cardId = -1;
            // 현재 칸(globalIndex)에 있는 카드 ID를 직접 가져옴
            if (globalIndex < currentDeck.Count)
            {
                cardId = currentDeck[globalIndex];
            }

            slot.Init(cardId, globalIndex, targetGrade);
            globalIndex++;
        }
    }
    // 등급별 슬롯 생성 헬퍼 함수
    private void CreateSlotsForGrade(CardGrade targetGrade, int count, ref List<int> currentDeckIds, ref int globalIndex)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject go = Instantiate(deckSlotPrefab, contentParent);
            DeckSlotUI slot = go.GetComponent<DeckSlotUI>();

            int equippedCardId = -1;
            // [안전장치] DataManager가 있는지 확인
            if (DataManager.Instance == null)
            {
                Debug.LogError("씬에 'DataManager'가 없습니다! 프리팹을 배치했는지 확인하세요.");
                return;
            }
            // 덱 리스트에서 이 등급에 맞는 카드를 찾음
            for (int j = 0; j < currentDeckIds.Count; j++)
            {
                int checkingId = currentDeckIds[j];
                var cardData = DataManager.Instance.GetCard(currentDeckIds[j]);
                // [안전장치] 카드 데이터가 제대로 로드되었는지 확인
                if (cardData == null)
                {
                    Debug.LogWarning($"[데이터 오류] ID가 {checkingId}인 카드 데이터를 찾을 수 없습니다. (CSV 로드 확인 필요)");
                    continue; // 이 카드는 건너뜀
                }
                bool isMatch = false;

                if (targetGrade == CardGrade.Epic)
                    isMatch = (cardData.CardGrade == CardGrade.Epic || cardData.CardGrade == CardGrade.Legendary);
                else
                    isMatch = (cardData.CardGrade == targetGrade);

                if (isMatch)
                {
                    equippedCardId = currentDeckIds[j];
                    currentDeckIds.RemoveAt(j);
                    break;
                }
            }

            slot.Init(equippedCardId, globalIndex, targetGrade);

            // 다음 슬롯을 위해 번호 증가
            globalIndex++;
        }
    }
}
