using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // 마우스 이벤트 필수

public class UIMonsterEffectIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image _iconImage;
    [SerializeField] private TextMeshProUGUI _valueText;

    private MonsterStatus _ownerStatus; // 누구의 효과인지

    // 초기화 함수
    public void Init(MonsterStatusEffectInstance data, MonsterStatus owner)
    {
        _ownerStatus = owner;

        // _iconImage.sprite = DataManager.Instance.GetStatusIcon(data.Img); 
        
        // 임시: 타입에 따라 색상만 변경 (이미지 리소스 적용 시 삭제)
        _iconImage.color = data.Type == BuffType.Buff ? Color.cyan : (data.Type == BuffType.DeBuff ? Color.magenta : Color.yellow);

        // 수치 표시 (지속시간 or 스택)
        int displayValue = Mathf.Max(data.Duration, data.Stack);
        _valueText.text = displayValue.ToString();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        // 몬스터가 가진 "모든" 효과 리스트를 툴팁에 전달
        // 위치는 현재 아이콘 혹은 몬스터의 위치 기준
        UIMonsterTooltip.Instance.ShowTooltip(_ownerStatus.StatusEffects, _ownerStatus.gameObject.transform.position);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UIMonsterTooltip.Instance.HideTooltip();
    }
}