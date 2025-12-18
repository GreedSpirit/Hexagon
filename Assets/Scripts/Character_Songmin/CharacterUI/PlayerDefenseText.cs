using TMPro;
using UnityEngine;

public class PlayerDefenseText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _defenseText;
    private void Awake()
    {
        if (_defenseText == null)
        {
            _defenseText = GetComponent<TextMeshProUGUI>();
        }
    }

    public void OnEnable()
    {
        if (Player.Instance != null)
        {
            Player.Instance.OnDefenseChanged += UpdateDefenseText;
            Player.Instance.PushDefense();
        }
        else
        {
            Debug.LogError("Player.Instance가 생성되지 않았습니다.");
        }
    }

    public void UpdateDefenseText(int defense)
    {
        _defenseText.text = $"{defense}";
    }

    public void OnDisable()
    {
        if (Player.Instance != null)
        {
            Player.Instance.OnDefenseChanged -= UpdateDefenseText;
        }
        else
        {
            Debug.LogError("Player.Instance가 생성되지 않았습니다.");
        }
    }
}
