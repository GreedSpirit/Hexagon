using UnityEngine;
using System.Collections.Generic;

public class InventoryTestSetter : MonoBehaviour
{
    private void Start()
    {
        if (CardManager.Instance == null || DataManager.Instance == null) return;

        Debug.Log("모든 카드를 인벤토리에 넣고, 가능한 한 덱에 꽉 채웁니다.");

        CardManager.Instance.UserCardList.Clear();
        CardManager.Instance.CurrentDeck.Clear();

        // 모든 카드 4장씩 획득
        foreach (var cardData in DataManager.Instance.CardDict.Values)
        {
            // (데이터상 IsCard가 다 false라서 조건문 없이 다 넣어야 함)
            CardManager.Instance.AddCard(cardData.Id, 4);
        }

        // 덱에 자동 장착
        foreach (var userCard in CardManager.Instance.UserCardList)
        {
            CardManager.Instance.ToggleDeckEquip(userCard.CardId);
        }

        // 저장 및 갱신
        GameSaveManager.Instance.SaveGame();

        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.RefreshInventory();
            InventoryManager.Instance.InvokeDeckChanged();
        }

        Debug.Log($" [설정 완료] 덱에 {CardManager.Instance.CurrentDeck.Count}장이 장착되었습니다.");
    }
}