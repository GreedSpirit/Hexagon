using UnityEngine;

public class InventoryTestSetter : MonoBehaviour
{
    private void Start()
    {
        if (CardManager.Instance == null) return;

        Debug.Log("<color=yellow>[Test] 강제로 덱 5장을 세팅합니다.</color>");


        CardManager.Instance.AddCard(1001, 10);
        CardManager.Instance.AddCard(1002, 10);
        CardManager.Instance.AddCard(1003, 10);

        // 덱 리스트 초기화 후 강제 주입

        CardManager.Instance.CurrentDeck.Clear();

        // 리스트에 직접 Add (ToggleDeckEquip 안 씀 -> 중복 로직 무시)
        CardManager.Instance.CurrentDeck.Add(1001);
        CardManager.Instance.CurrentDeck.Add(1002);
        CardManager.Instance.CurrentDeck.Add(1003);
        CardManager.Instance.CurrentDeck.Add(1001); // 강제 중복 1
        CardManager.Instance.CurrentDeck.Add(1002); // 강제 중복 2

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