using TMPro;
using UnityEngine;

public class CardTooltipUI : MonoBehaviour
{
    [Header("UI 요소")]
    [SerializeField] TextMeshProUGUI _statusName;
    [SerializeField] TextMeshProUGUI _statusDesc;

    // 툴팁 텍스트 설정
    public void SetToolTipText(string name, string desc)
    {
        _statusName.text = name;
        _statusDesc.text = desc;
    }
}
