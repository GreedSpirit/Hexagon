using UnityEngine;

public class PlayerLevelUpUI : MonoBehaviour
{
    [SerializeField] GameObject _levelUpUIPrefab;    

    void Start()
    {
        if (Player.Instance != null)
        {
            Player.Instance.OnLevelChanged += ShowLevelUpUI;
        }            
    }

    private void OnDestroy()
    {
        if (Player.Instance != null)
        {
            Player.Instance.OnLevelChanged -= ShowLevelUpUI;
        }            
    }

    void ShowLevelUpUI(int level)
    {
        if (level == 1)
        {
            return;
        }
        PlayerLevelUpObject popup =  Instantiate(_levelUpUIPrefab, transform).GetComponent<PlayerLevelUpObject>();
        popup.Init(level);        
    }
}
