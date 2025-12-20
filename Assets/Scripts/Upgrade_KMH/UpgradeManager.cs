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
        foreach(int cardId in CardManager.Instance.CurrentDeck)
        {
            Button button = Instantiate(_upgradeButtonPrefab, _upgradeButton);
            button.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = $"{cardId} Card Upgrade";
            button.onClick.AddListener(() => TryUpgradeCard(cardId, button));
        }
    }


    // 카드 강화 시도
    public void TryUpgradeCard(int cardId, Button button)
    {
        bool isUpgrade = CardManager.Instance.TryUpgradeCard(cardId);
    }
}
