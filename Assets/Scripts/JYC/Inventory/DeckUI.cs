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

    private void Start()
    {
        // 매니저의 이벤트 구독
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnDeckChanged += RefreshDeck;

            // 시작하자마자 한 번 갱신
            RefreshDeck();
        }
    }

    private void OnDestroy()
    {
        // 구독 해제 
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnDeckChanged -= RefreshDeck;
        }
    }

    // 덱 리스트 새로고침
    public void RefreshDeck()
    {
        // 슬롯 초기화
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        // 원본 리스트 가져오기
        List<int> originalDeck = InventoryManager.Instance.CurrentDeck;

        // 정렬된 '복사본' 만들기
        var sortedList = originalDeck
            .OrderBy(id =>
            {
                var data = DataManager.Instance.GetCard(id);
                return data != null ? data.CardGrade : CardGrade.Common;
            }) // 1순위: 등급
            .ThenBy(id =>
            {
                var data = DataManager.Instance.GetCard(id);
                return data != null ? data.Name : "";
            }) // 2순위: 이름
            .ToList();

        // 원본 리스트의 '내용'만 정렬된 순서로 업데이트
        originalDeck.Clear();
        originalDeck.AddRange(sortedList);

        // 이제 정렬된 원본 리스트로 슬롯 생성
        for (int i = 0; i < originalDeck.Count; i++)
        {
            int cardId = originalDeck[i];

            GameObject go = Instantiate(deckSlotPrefab, contentParent);
            DeckSlotUI slot = go.GetComponent<DeckSlotUI>();

            // ID와 순서(Index) 넘겨주기
            slot.Init(cardId, i);
        }

        // 텍스트 갱신
        if (countText != null)
        {
            int maxCount = CardManager.MAX_DECK_COUNT;
            countText.text = $"Deck : {originalDeck.Count} / {maxCount}";
        }
    }
    // [전투 시작] 버튼에 연결할 함수
    public void OnClickStartGame()
    {
        // [TODO] 기획서 규칙: 던전마다 요구 카드 수가 다름.
        // 현재 선택된 던전 정보를 가져옵니다.
        // DungeonPresenter나 Manager 어딘가에 선택된 던전 ID
        // 예: int currentDungeonId = DungeonManager.Instance.SelectedDungeonId;

        // 그 던전의 데이터에서 '필요 개수'를 꺼내기
        // DungeonData dungeonData = DataManager.Instance.GetDungeon(currentDungeonId);
        // int limit = dungeonData.RequiredCardCount; // 데이터에서 가져온 진짜 숫자 (5, 6, 8...)

        // 임시 조치
        int limit = 5; // 나중에 위 주석 풀어서 limit에 넣으면 됨

        // 유효성 검사
        if (!CardManager.Instance.IsDeckValid(limit))
        {
            Debug.Log($"이 던전은 {limit}종류의 카드가 필요합니다! 출발 불가");
            return;
        }

        // 덱 저장 (출발 전 자동 저장)
        CardManager.Instance.SaveGame();

        // 진짜 전투 씬 로드
        SceneManager.LoadScene("BattleScene");
    }
}