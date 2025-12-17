using UnityEngine;
using UnityEngine.UI;

public class PlayerHpBar : MonoBehaviour
{
    [SerializeField] private Slider _hpBar;
    [SerializeField] private Slider _poisonBar;
    [SerializeField] private Slider _burnBar;
    

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

    public void UpdateHpBar(int currentHp, int Hp, int poison, int burn)
    {
        if (currentHp != 0)
        {
            _hpBar.value = Mathf.Max(0, (float) (currentHp - poison - burn) / Hp);
            _burnBar.value = Mathf.Max(0, (float)(currentHp - poison) / Hp);
            _poisonBar.value = Mathf.Max(0, (float)currentHp / Hp);
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
