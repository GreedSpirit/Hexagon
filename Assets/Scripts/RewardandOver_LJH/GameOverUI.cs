using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private GameObject _gameOverPanel;
    public void Start()
    {        
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
        else
        {
            Debug.LogError("Player.Instance가 생성되지 않았습니다.");
        }
    }


    public void CheckGameOver(int currentHp, int Hp, int poison, int burn)
    {
        if(currentHp <= 0)
        {
            Player.Instance.EnterReward();
            _gameOverPanel.SetActive(true);
        }
    }   

}
