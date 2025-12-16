using UnityEngine;
using UnityEngine.UI;

public class PlayerHpBar : MonoBehaviour
{
    [SerializeField] private Slider _hpBar;

    private void Awake()
    {
        if (_hpBar == null)
        {
            _hpBar = GetComponent<Slider>();
        }
    }

    public void OnEnable()
    {                
        if (Player.Instance != null)
        {
            Player.Instance.OnHpChanged += UpdateHpBar;
            Player.Instance.PushHp();
        }
        else
        {
            Debug.LogError("Player.Instance가 생성되지 않았습니다.");
        }
        
    }

    public void UpdateHpBar(int currentHp, int Hp)
    {
        if (currentHp != 0)
        {
            _hpBar.value = (float)currentHp / Hp;
        }
        else
        {
            _hpBar.value = 0;
        }
        
    }

    public void OnDisable()
    {
        if (Player.Instance != null)
        {
            Player.Instance.OnHpChanged -= UpdateHpBar;
        }
        else
        {
            Debug.LogError("Player.Instance가 생성되지 않았습니다.");
        }
    }
}
