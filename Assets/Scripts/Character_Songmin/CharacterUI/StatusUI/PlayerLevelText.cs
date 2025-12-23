using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerLevelText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _levelText;    

    public void Start()
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

    public void OnDestroy()
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

    public void UpdateLevelText(int level)
    {
        _levelText.text = $"Lv {level}";
    }
}
