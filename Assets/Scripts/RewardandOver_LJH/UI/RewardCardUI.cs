using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // 마우스 이벤트 필수
using TMPro;
using System.Text;

public class RewardCardUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Visual Components")]
    [SerializeField] private Transform _visualTransform; // 확대/축소할 대상
    [SerializeField] private GameObject _selectedEdge;   // 호버 시 테두리

    [Header("Card UI Elements")]
    [SerializeField] private Image _img;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _descText;
    [SerializeField] private TextMeshProUGUI _typeText;
    [SerializeField] private TextMeshProUGUI _levelText;

    [SerializeField] private TextMeshProUGUI _amountText;
    
    [Header("Grade Settings")]
    [SerializeField] private Image _gradeColorImg;
    [SerializeField] private Color[] _gradeColors; 

    [Header("Hover Settings")]
    [SerializeField] private float _hoverScale = 1.2f; // 호버 시 배율
    [SerializeField] private float _scaleSpeed = 10f;  // 커지는 속도

    private CardData _cardData;
    private Vector3 _targetScale;

    private void Awake()
    {
        // 초기 스케일 설정
        _targetScale = Vector3.one;
        if (_visualTransform == null) _visualTransform = transform;
    }

    private void Update()
    {
        _visualTransform.localScale = Vector3.Lerp(_visualTransform.localScale, _targetScale, Time.deltaTime * _scaleSpeed);
    }

    // RewardResultUI에서 호출
    public void Init(CardData data, int amount = 1)
    {
        _cardData = data;
        UpdateVisual(amount);
    }

    private void UpdateVisual(int amount)
    {
        if (_cardData == null) return;


        if (_img != null) _img.sprite = DataManager.Instance.GetCardSprite(_cardData.CardImg);
        if (_nameText != null) _nameText.text = _cardData.Name;
        if (_descText != null) UpdateDesc();
        if (_levelText != null) _levelText.text = "1";

        if(_amountText != null)
        {
            _amountText.text = $"X {amount}";
        }

        SetTypeText();

        SetGradeColor();

        if (_selectedEdge != null) _selectedEdge.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _targetScale = Vector3.one * _hoverScale;

        if (_selectedEdge != null) _selectedEdge.SetActive(true);

        //transform.SetAsLastSibling();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _targetScale = Vector3.one;

        if (_selectedEdge != null) _selectedEdge.SetActive(false);
    }
    private void SetTypeText()
    {
        if (_typeText == null) return;

        string typeName = "기타";
        switch (_cardData.CardType)
        {
            case CardType.Attack: typeName = "공격"; break;
            case CardType.Shield: typeName = "방어"; break;
            case CardType.Healing: typeName = "치유"; break;
            case CardType.Spell: typeName = "주문"; break;
        }
        _typeText.text = typeName;
    }

    private void SetGradeColor()
    {
        if (_gradeColorImg == null || _gradeColors == null || _gradeColors.Length == 0) return;

        int gradeIndex = (int)_cardData.CardGrade - 1;
        if (gradeIndex >= 0 && gradeIndex < _gradeColors.Length)
        {
            _gradeColorImg.color = _gradeColors[gradeIndex];
        }
    }

    public void UpdateDesc()
    {
        // 카드 설명 있을 때만
        if (string.IsNullOrEmpty(_cardData.Desc)) return;

        // 문자열 갱신
        StringBuilder sb = new StringBuilder(_cardData.Desc);

        sb.Replace("{D}", _cardData.BaseValue.ToString());
        sb.Replace("{N}", _cardData.BaseValue.ToString());
        sb.Replace("{SEV}", _cardData.StatusEffectValue.ToString());
        sb.Replace("{turns}", _cardData.Turn.ToString());

        _descText.text = sb.ToString();
    }
}