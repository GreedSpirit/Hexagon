using TMPro;
using UnityEngine;

public class PlayerMoneyText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _moneyText;


    public void Start()
    {
        if (Player.Instance != null)
        {
            Player.Instance.OnMoneyChanged += UpdateMoneyText;
            Player.Instance.PushMoney();
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
            Player.Instance.OnMoneyChanged -= UpdateMoneyText;
        }
        else
        {
            Debug.LogError("Player.Instance가 생성되지 않았습니다.");
        }
    }


    public void UpdateMoneyText(int money)
    {
        _moneyText.text = money.ToString("N0");
    }
}
