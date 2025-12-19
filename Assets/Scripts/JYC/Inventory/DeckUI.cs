using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

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
        // 기존 슬롯 싹 지우기
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        // 현재 덱 리스트 가져오기
        List<int> currentDeckIds = InventoryManager.Instance.CurrentDeck;

        // 정렬: 일반 -> 희귀 -> 영웅 -> 전설 (그 다음 이름순)
        var sortedDeck = currentDeckIds
            .Select(id => DataManager.Instance.GetCard(id))
            .OrderBy(data => data.CardGrade) // Enum 순서(0~3)대로 정렬됨
            .ThenBy(data => data.Name)       // 같은 등급이면 이름순
            .ToList();

        // 슬롯 생성
        foreach (var cardData in sortedDeck)
        {
            GameObject go = Instantiate(deckSlotPrefab, contentParent);
            DeckSlotUI slot = go.GetComponent<DeckSlotUI>();

            // ID를 넘겨주면서 초기화
            slot.Init(cardData.Id);
        }

        // 텍스트 갱신 (예: Deck : 5 / 30)
        if (countText != null)
        {
            countText.text = $"Deck : {sortedDeck.Count} / 30";
        }
    }
}