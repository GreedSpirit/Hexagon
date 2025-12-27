using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RewardResultUI : MonoBehaviour
{
    [Header("Left Section (Boss Card)")]
    [SerializeField] private GameObject _bossSection;
    [SerializeField] private Transform _bossCardSpawnPoint;

    [Header("Right Section (General Card)")]
    [SerializeField] private Transform _generalCardParent; // Horizontal Layout Group 위치
    [SerializeField] private GameObject _cardUIPrefab;

    [Header("Right Section (Currency)")]
    [SerializeField] private Transform _currencyParent;    // Vertical Layout Group 위치
    [SerializeField] private GameObject _currencyTextPrefab; // 텍스트만 있는 프리팹
    [SerializeField] private GameObject _currencyIsOverText;

    [Header("Buttons")]
    [SerializeField] private Button _returnButton;

    private DungeonManager _dungeonManager; // GetRewards 호출용 참조

    public void Init(List<DeterminedReward> rewards, DungeonManager manager)
    {
        _dungeonManager = manager;
        _returnButton.onClick.RemoveAllListeners();
        _returnButton.onClick.AddListener(OnRewardReturnButtonClicked);

        // UI 초기화 (기존 데이터 삭제)
        ClearUI();

        foreach (var reward in rewards)
        {
            switch (reward.RewardType)
            {
                case "BossCard":
                    SetBossCard(reward);
                    break;

                case "Card":
                    CreateGeneralCardSlot(reward);
                    break;

                case "Currency":
                    CreateCurrencyText(reward);
                    break;
            }
        }
    }

    private void SetBossCard(DeterminedReward reward)
    {
        CardData data = DataManager.Instance.GetCard(reward.ItemId);

        if(data != null)
        {
            GameObject obj = Instantiate(_cardUIPrefab, _bossCardSpawnPoint);
            obj. transform.localPosition = Vector3.zero;
            obj.transform.localScale = Vector3.one * 1.5f;

            RewardCardUI cardUI = obj.GetComponent<RewardCardUI>();
            cardUI.Init(data,reward.Amount);
        }
    }

    private void CreateGeneralCardSlot(DeterminedReward reward)
    {
        CardData data = DataManager.Instance.GetCard(reward.ItemId);

        GameObject obj = Instantiate(_cardUIPrefab, _generalCardParent);
        RewardCardUI cardUI = obj.GetComponent<RewardCardUI>();
        
        cardUI.Init(data,reward.Amount);
    }

    private void CreateCurrencyText(DeterminedReward reward)
    {
        GameObject obj = Instantiate(_currencyTextPrefab, _currencyParent);
        TextMeshProUGUI textComp = obj.GetComponent<TextMeshProUGUI>();

        // 텍스트 포맷: "아이템 이름 : 수량"
        // reward.ItemKey를 이용해 실제 한글 이름을 가져오는 로직 필요
        string displayName = DataManager.Instance.GetString(reward.ItemKey).Korean; 
        
        textComp.text = $"{displayName} : {reward.Amount}";
    }

    public void OnMoneyOverText()
    {
        _currencyIsOverText.SetActive(true);
    }

    private void ClearUI()
    {
        foreach (Transform child in _generalCardParent) Destroy(child.gameObject);
        foreach (Transform child in _currencyParent) Destroy(child.gameObject);
        foreach(Transform child in _bossCardSpawnPoint) Destroy(child.gameObject);
        
        _currencyIsOverText.SetActive(false);
    }

    private void OnReturnButtonClicked()
    {
        Player.Instance.EnterMoveMod();
        Player.Instance.Respawn();

        SceneManager.LoadScene(0);
        //SceneTransitionManager.Instance.LoadSceneWithFade(씬 이름)
    }

    private void OnRewardReturnButtonClicked()
    {
        //실제 보상 획득 로직 실행
        _dungeonManager.GetRewards();

        // [추가] 보상을 다 받았으니 저장
        GameSaveManager.Instance.SaveGame();
        Debug.Log("보상 획득 및 저장 완료");

        OnReturnButtonClicked();
    }
}