using UnityEngine;
using System.Collections.Generic;

public class InventoryTestSetter : MonoBehaviour
{
    private void Start()
    {
        // 매니저 존재 여부 확인 (안전장치)
        if (CardManager.Instance == null || DataManager.Instance == null)
        {
            Debug.LogError(" 매니저(CardManager 또는 DataManager)가 씬에 없습니다!");
            return;
        }

        Debug.Log(" 데이터 테이블의 '모든 카드'를 인벤토리에 추가합니다.");

        // 기존 인벤토리 및 덱 초기화 (중복 방지)
        // UserCardList를 싹 비우고 시작합니다.
        CardManager.Instance.UserCardList.Clear();
        CardManager.Instance.CurrentDeck.Clear();

        // DataManager에 로드된 모든 카드 데이터를 순회하며 추가
        // 엑셀(CSV)에 있는 모든 카드를 하나씩 가져옵니다.
        foreach (var cardData in DataManager.Instance.CardDict.Values)
        {
            // IsCard가 false 밖에 없기에 테스트를 위해 일단 다 넣습니다.

            // 모든 카드를 3장씩 지급
            CardManager.Instance.AddCard(cardData.Id, 3);
        }

        // 덱에 일부 카드 자동 장착 테스트
        // 앞에서부터 5장만 자동으로 덱에 넣어둡니다. (테스트 편의용)
        int autoEquipCount = 0;
        foreach (var card in CardManager.Instance.UserCardList)
        {
            if (autoEquipCount >= 5) break; // 5장만 넣고 그만

            // 덱에 장착 시도 (성공하면 true 반환)
            if (CardManager.Instance.ToggleDeckEquip(card.CardId))
            {
                autoEquipCount++;
            }
        }

        // 저장 및 UI 즉시 갱신
        CardManager.Instance.SaveGame();

        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.RefreshInventory(); // 인벤토리 UI 갱신
            InventoryManager.Instance.InvokeDeckChanged(); // 덱 UI 갱신
        }

        Debug.Log($" [Test 완료] 총 {CardManager.Instance.UserCardList.Count}종류의 카드가 지급되었습니다.");
    }
}