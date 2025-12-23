using UnityEngine;

public class InventoryTestSetter : MonoBehaviour
{
    private void Start()
    {
        if (CardManager.Instance == null) return;

        Debug.Log("<color=yellow>[Test] 강제로 덱 5장을 세팅합니다.</color>");

        CardManager.Instance.AddCard(1, 10);
        CardManager.Instance.AddCard(2, 10);
        CardManager.Instance.AddCard(3, 10);

        // 덱 리스트 초기화 후 강제 주입
        CardManager.Instance.CurrentDeck.Clear();

        CardManager.Instance.CurrentDeck.Add(1);
        CardManager.Instance.CurrentDeck.Add(2);
        CardManager.Instance.CurrentDeck.Add(3);
        CardManager.Instance.CurrentDeck.Add(1); // 강제 중복 1
        CardManager.Instance.CurrentDeck.Add(2); // 강제 중복 2

        // 강제 저장 (이 상태를 박제)
        CardManager.Instance.SaveGame();

        // UI 새로고침
        if (InventoryManager.Instance != null)
        {
            // 인벤토리 UI 갱신
            InventoryManager.Instance.RefreshInventory();
            // 덱 UI 갱신 (DeckUI가 이 이벤트를 구독하고 있어야 함)
            InventoryManager.Instance.InvokeDeckChanged();
        }

        Debug.Log($"[Test] 설정 완료. 현재 덱: {CardManager.Instance.CurrentDeck.Count}장");
    }
}