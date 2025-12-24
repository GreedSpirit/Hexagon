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
    // 현재 선택된 던전 데이터
    private DungeonData _currentDungeon;
    private void Start()
    {
        
        // 매니저의 이벤트 구독
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnDeckChanged += RefreshDeck;

            // 시작하자마자 한 번 갱신
            RefreshDeck();

        }
        // 시작할 때 팝업은 꺼두기
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

        // DeckUI를 켭니다.
        this.gameObject.SetActive(true);
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
    // '뒤로가기'나 '닫기' 버튼에 연결할 함수 (마을로 돌아갈 때)
    public void CloseDeckUI()
    {
        // DeckUI 끄기
        this.gameObject.SetActive(false);

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
        CardManager.Instance.SaveGame();

        // 진짜 씬 이동
        string dungeonName = _targetDungeon != null ? _targetDungeon.Name : "Unknown Dungeon";
        Debug.Log($"던전 '{dungeonName}'으로 출발합니다!");

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

        List<int> currentDeckIds = new List<int>(InventoryManager.Instance.CurrentDeck);

        // 전체 슬롯의 순서를 세기 위한 변수 (0, 1, 2, 3...)
        int globalSlotIndex = 0;

        // 등급별 슬롯 생성 (ref로 globalSlotIndex를 넘겨서 계속 증가시킴)
        CreateSlotsForGrade(CardGrade.Common, requiredDeck.NormalCount, ref currentDeckIds, ref globalSlotIndex);
        CreateSlotsForGrade(CardGrade.Rare, requiredDeck.RareCount, ref currentDeckIds, ref globalSlotIndex);
        CreateSlotsForGrade(CardGrade.Epic, requiredDeck.EpicCount, ref currentDeckIds, ref globalSlotIndex);

        if (countText != null)
        {
            int totalRequired = requiredDeck.NormalCount + requiredDeck.RareCount + requiredDeck.EpicCount;
            int currentCount = InventoryManager.Instance.CurrentDeck.Count;
            countText.text = $"Deck : {currentCount} / {totalRequired}";
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
