using UnityEngine;

public class InventoryTestSetter : MonoBehaviour
{
    private void Start()
    {
        if (CardManager.Instance == null)
        {
            // CardManager는 DontDestroyOnLoad라서 보통 살아있지만 혹시 모르니 체크
            Debug.LogError("CardManager가 없습니다!");
            return;
        }

        if (CardManager.Instance.UserCardList.Count > 0)
        {
            Debug.Log("[Test] 기존 데이터가 있어 테스트 데이터 생성을 건너뜁니다.");
            return;
        }

        Debug.Log("[Test] 테스트 데이터를 생성합니다.");
        CardManager.Instance.AddCard(1001, 1);
        CardManager.Instance.AddCard(1002, 1); 
        CardManager.Instance.AddCard(1003, 1); 

        if (CardManager.Instance.CurrentDeck.Count == 0)
        {
            CardManager.Instance.ToggleDeckEquip(1);
            CardManager.Instance.ToggleDeckEquip(3);
        }

        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.RefreshInventory();
        }
    }
}
