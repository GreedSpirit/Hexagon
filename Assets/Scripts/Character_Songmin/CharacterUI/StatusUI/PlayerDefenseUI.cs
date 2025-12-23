using TMPro;
using UnityEngine;

public class PlayerDefenseText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _defenseText;
    

    public void Start()
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

    public void OnDestroy()
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


    public void UpdateDefenseText(int defense)
    {
        _defenseText.text = $"{defense}";
    }

}
