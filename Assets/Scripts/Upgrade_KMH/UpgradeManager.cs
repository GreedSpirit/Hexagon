using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;

    [SerializeField] Button _upgradeButtonPrefab;// 업그레이트 버튼 프리팹
    [SerializeField] Transform _upgradeButton;  // 업그레이드 버튼 패널

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void Start()
    {
        // 강화 시도 테스트용 버튼 카드마다 생성
        foreach(int cardId in TestGameManager_KMH.Instance.Deck.Keys)
        {
            Button button = Instantiate(_upgradeButtonPrefab, _upgradeButton);
            button.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = $"{cardId} Card Upgrade";
            button.onClick.AddListener(() => TryUpgradeCard(cardId, button));
        }
    }


    // 카드 강화 시도
    public void TryUpgradeCard(int cardId, Button button)
    {
        // Deck이 지금은 일단 전체 카드 리스트긴 한데
        // 나중에 GameManager나 CardManager 제대로 만들어서 InventoryManager랑 같이 사용하면 될듯?
        if (TestGameManager_KMH.Instance.Deck.ContainsKey(cardId) == false)
        {
            Debug.LogError("강화하려는 카드가 인벤토리에 없습니다.");

            return;
            //return false;
        }

        int level = TestGameManager_KMH.Instance.Deck[cardId];

        // 재화 체크 (카드 재료 수)

        // 레벨 증가
        level++;

        TestGameManager_KMH.Instance.Deck[cardId] = level;

        Debug.Log($"{cardId}번 카드 강화 성공. 현재 레벨: {level}");

        // 강화 최대 레벨 체크 (임시 최대 레벨 5)
        if (level >= 5)
        {
            button.interactable = false;
            Debug.Log("최대 레벨입니다.");
        }
        //return true;
    }
}
