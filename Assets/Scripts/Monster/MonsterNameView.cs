using TMPro;
using UnityEngine;

public class MonsterNameView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _monsterNameText;

    public void Init()
    {
        _monsterNameText.gameObject.SetActive(false);
    }

    public void SetName(string name)
    {
        _monsterNameText.text = name;
    }

    public void HoverNameView(bool isHover)
    {
        _monsterNameText.gameObject.SetActive(isHover);
    }
}
