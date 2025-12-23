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

    [Header("Dungeon Info UI")]
    [SerializeField] TextMeshProUGUI _dungeonNameText;
    [SerializeField] TextMeshProUGUI _dungeonDescText;
    // [SerializeField] Image _dungeonImage; // 이미지도 있다면

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
    // DungeonPresenter가 호출해줄 함수
    public void ReadyForBattle(DungeonData dungeon)
    {
        _targetDungeon = dungeon;

        // 받아온 던전 정보로 텍스트 갱신
        if (_dungeonNameText != null) _dungeonNameText.text = dungeon.Name;
        if (_dungeonDescText != null) _dungeonDescText.text = dungeon.Desc;

        RefreshDeck();
    }
    // [전투 시작] 버튼에 연결할 최종 함수
    public void OnClickStartBattle()
    {
        // 덱 유효성 검사
        // 기획: 던전마다 요구 카드 수가 다를 수 있음 (현재는 임시 5장)
        // 추후 _targetDungeon.CardLimit 등으로 교체 가능
        int requiredCount = 5;

        if (!CardManager.Instance.IsDeckValid(requiredCount))
        {
            Debug.Log($"덱이 완성되지 않았습니다! ({requiredCount}장 필요)");
            // 여기에 유저에게 알리는 팝업 메시지
            return;
        }

        // 최종 저장 (전투 진입 전 상태 저장)
        CardManager.Instance.SaveGame();

        // 진짜 씬 이동
        // _targetDungeon이 null일 경우에 대한 방어 코드 추가
        string dungeonName = _targetDungeon != null ? _targetDungeon.Name : "Unknown Dungeon";
        Debug.Log($"던전 '{dungeonName}'으로 출발합니다!");

        // 실제 전투 씬 이름으로 로드 (기획서나 프로젝트 설정에 맞게 변경)
        SceneManager.LoadScene("DungeonBattleScene");
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
    
}