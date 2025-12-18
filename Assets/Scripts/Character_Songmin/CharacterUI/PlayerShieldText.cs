using TMPro;
using UnityEngine;

public class PlayerShieldText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _shieldText;
    private void Awake()
    {
        if (_shieldText == null)
        {
            _shieldText = GetComponent<TextMeshProUGUI>();
        }
    }

    public void OnEnable()
    {
        if (Player.Instance != null)
        {
            Player.Instance.OnShieldChanged += UpdateShieldText;
            Player.Instance.PushDefense();
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

    public void OnDisable()
    {
        if (Player.Instance != null)
        {
            Player.Instance.OnShieldChanged -= UpdateShieldText;
        }
        else
        {
            Debug.LogError("Player.Instance가 생성되지 않았습니다.");
        }
    }
}
