using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerLevelText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _levelText;
    private void Awake()
    {
        if (_levelText == null)
        {
            _levelText = GetComponent<TextMeshProUGUI>();
        }
    }

    public void OnEnable()
    {
        if (Player.Instance != null)
        {
            Player.Instance.OnLevelChanged += UpdateLevelText;
            Player.Instance.PushLevel();
        }
        else
        {
            Debug.LogError("Player.Instance가 생성되지 않았습니다.");
        }
    }

    public void UpdateLevelText(int level)
    {
        _levelText.text = $"Lv {level}";
    }

    public void OnDisable()
    {
        if (Player.Instance != null)
        {
            Player.Instance.OnLevelChanged -= UpdateLevelText;
        }
        else
        {
            Debug.LogError("Player.Instance가 생성되지 않았습니다.");
        }
    }    
}
