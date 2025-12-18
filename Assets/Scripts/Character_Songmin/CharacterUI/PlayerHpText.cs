using TMPro;
using UnityEngine;

public class PlayerHpText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _hpText;
    private void Awake()
    {
        if (_hpText == null)
        {
            _hpText = GetComponent<TextMeshProUGUI>();
        }
    }

    public void OnEnable()
    {        
        if (Player.Instance != null)
        {
            Player.Instance.OnHpChanged += UpdateHpText;
            Player.Instance.PushHp();
        }
        else
        {
            Debug.LogError("Player.Instance가 생성되지 않았습니다.");
        }
    }

    public void UpdateHpText(int currentHp, int Hp, int poison, int burn)
    {
        _hpText.text = $"{currentHp}  /  {Hp}";
    }

    public void OnDisable()
    {
        if (Player.Instance != null)
        {
            Player.Instance.OnHpChanged -= UpdateHpText;
        }
        else
        {
            Debug.LogError("Player.Instance가 생성되지 않았습니다.");
        }
    }
}
