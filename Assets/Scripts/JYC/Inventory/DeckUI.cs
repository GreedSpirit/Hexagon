using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.SceneManagement;

public class DeckUI : MonoBehaviour
{

    [SerializeField] Transform contentParent;   // Scroll View의 Content
    [SerializeField] GameObject deckSlotPrefab; // DeckSlotUI가 붙은 프리팹
    [SerializeField] TextMeshProUGUI countText;

    private DungeonData _targetDungeon;

    [Header("Confirmation Popup")]
    [SerializeField] GameObject _enterPopupPanel; // 팝업 패널 전체
    [SerializeField] TextMeshProUGUI _popupText;  // 팝업 내용 텍스트

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

        RefreshDeck();
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

            // 덱 리스트에서 이 등급에 맞는 카드를 찾음
            for (int j = 0; j < currentDeckIds.Count; j++)
            {
                var cardData = DataManager.Instance.GetCard(currentDeckIds[j]);
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
