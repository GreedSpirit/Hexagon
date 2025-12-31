using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private GameObject _gameOverPanel;
    BattleManager _battleManager;

    public void Start()
    {        
        _battleManager = FindFirstObjectByType<BattleManager>();
        if (Player.Instance != null)
        {
            Player.Instance.OnHpChanged += CheckGameOver;
        }
        else
        {
            Debug.LogError("Player.Instance가 생성되지 않았습니다.");
        }
    }

    public void OnDestroy()
    {
        if (Player.Instance != null)
        {
            Player.Instance.OnHpChanged -= CheckGameOver;
        }
    }


    public void CheckGameOver(int currentHp, int Hp, int poison, int burn)
    {
        if(currentHp <= 0)
        {
            GameOver();
        }
    }

    public void GameOver()
    {
        Player.Instance.EnterReward();
        _battleManager.OnOffBattleUI(false);
        _battleManager.OffTurnUI();
        _gameOverPanel.SetActive(true);
    }

}
