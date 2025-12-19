using TMPro;
using UnityEngine;

public class PlayerShieldUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _shieldText;
    [SerializeField] GameObject _shieldObject;
    

    public void Start()
    {
        if (Player.Instance != null)
        {
            Player.Instance.OnShieldChanged += UpdateShieldText;
            Player.Instance.OnShieldChanged += OnOffShieldUI;
            Player.Instance.PushShield();
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
            Player.Instance.OnShieldChanged -= UpdateShieldText;
            Player.Instance.OnShieldChanged -= OnOffShieldUI;
        }
        else
        {
            Debug.LogError("Player.Instance가 생성되지 않았습니다.");
        }
    }

    public void UpdateShieldText(int shield)
    {
        _shieldText.text = $"{shield}";
    }

    public void OnOffShieldUI(int shield)
    {
        if (shield <= 0)
        {
            _shieldObject.SetActive(false);
        }
        else
        {
            _shieldObject.SetActive(true);
        }
    }    
}
