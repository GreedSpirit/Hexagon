using TMPro;
using UnityEngine;

public class PlayerExpText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _expText;
    

    public void Start()
    {
        if (Player.Instance != null)
        {
            Player.Instance.OnExpChanged += UpdateExpText;
            Player.Instance.PushExp();
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
            Player.Instance.OnExpChanged -= UpdateExpText;
        }
        else
        {
            Debug.LogError("Player.Instance가 생성되지 않았습니다.");
        }
    }

    public void UpdateExpText(int currentExp, int exp)
    {
        _expText.text = $"{currentExp}  /  {exp}";
    }
}
