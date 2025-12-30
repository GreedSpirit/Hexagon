using UnityEngine;

public class PlayerLevelUpUI : MonoBehaviour
{
    [SerializeField] GameObject _levelUpUIPrefab;

    int _lastLevel = 1;

    void Start()
    {
        if (Player.Instance != null)
        {
            _lastLevel = Player.Instance.GetLevel();
            Player.Instance.OnLevelUp += ShowLevelUpUI;
        }            
    }

    private void OnDestroy()
    {
        if (Player.Instance != null)
        {
            Player.Instance.OnLevelUp -= ShowLevelUpUI;
        }            
    }

    void ShowLevelUpUI(int level)
    {
        if (level <= _lastLevel)
        {
            return;
        }            

        _lastLevel = level;

        PlayerLevelUpObject popup = Instantiate(_levelUpUIPrefab, transform).GetComponent<PlayerLevelUpObject>();

        popup.Init(level);
    }
}
