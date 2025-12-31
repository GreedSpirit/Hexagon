using UnityEngine;
using UnityEngine.UI;

public class CardTooltipLogic : MonoBehaviour
{
    [Header("툴팁 위치")]
    [SerializeField] Transform tooltipPoint;

    private Transform _baseParent;      // 기본 부모

    private CardTooltipUI _tooltipUI;   // 툴팁 UI

    private string _name;               // 효과 이름
    private string _desc;               // 효과 설명

    private void Start()
    {
        if (_tooltipUI != null)
            _baseParent = _tooltipUI.transform.parent;
    }

    // CardData
    public void Init(CardData data, CardTooltipUI tooltipUI)
    {
        _tooltipUI = tooltipUI;

        SetString(data);
    }

    //UserCard
    public void Init(UserCard userCard, CardTooltipUI tooltipUI)
    {
        _tooltipUI = tooltipUI;

        CardData data = DataManager.Instance.GetCard(userCard.CardId);

        SetString(data);
    }


    private void SetString(CardData data)
    {
        // 효과가 있을 때
        if (string.IsNullOrEmpty(data.StatusEffect) == false)
        {
            // 효과 데이터
            StatusEffectData statusEffectData = DataManager.Instance.GetStatusEffectData(data.StatusEffect);
            // 이름 스트링
            StringData nameStringData = DataManager.Instance.GetString(statusEffectData.Name);
            // 설명 스트링
            StringData descStringData = DataManager.Instance.GetString(statusEffectData.Desc);
            // 이름 할당
            _name = nameStringData.Korean;
            // 설명 할당
            _desc = descStringData.Korean;
        }
    }

    public void PointerEnterParent()
    {
        // 툴팁이 없거나 설명 비어있으면 스킵
        if (_tooltipUI == null || string.IsNullOrEmpty(_desc)) return;

        // 활성화
        _tooltipUI.gameObject.SetActive(true);

        // 부모 변경
        _tooltipUI.transform.SetParent(tooltipPoint, false);

        // 로컬 위치 0,0,0
        _tooltipUI.transform.localPosition = Vector3.zero;

        // 툴팁 텍스트 변경
        _tooltipUI.SetToolTipText(_name, _desc);


        // 레이아웃 강제 갱신
        // LayoutGroup(Grid/Vertical) 즉시 다시 계산
        LayoutRebuilder.ForceRebuildLayoutImmediate(tooltipPoint.GetComponent<RectTransform>());
    }

    public void PointerExitParent()
    {
        if (_tooltipUI == null) return;
        // 비활성화
        _tooltipUI?.gameObject.SetActive(false);

        // 다시 원래 부모로
        _tooltipUI.transform.SetParent(_baseParent, false);

        // 위치 꼬일 수도 있으니까
        _tooltipUI.transform.localPosition = Vector3.zero;
    }

    // 추가 코드
    private void OnDisable()
    {
        // 카드가 사라지기 전에 툴팁을 복구시킵니다.
        ReturnTooltipToSafety();
    }
    private void ReturnTooltipToSafety()
    {
        // 툴팁 UI가 이미 파괴되었거나(Missing) 없는 경우를 체크
        if (_tooltipUI == null) return;

        // 툴팁이 현재 이 카드의 자식으로 붙어있을 때만 복구 로직 실행
        if (_tooltipUI.transform.parent == tooltipPoint)
        {
            _tooltipUI.gameObject.SetActive(false);
            if (_baseParent != null)
            {
                _tooltipUI.transform.SetParent(_baseParent, false);
            }
            _tooltipUI.transform.localPosition = Vector3.zero;
        }
    }

}
