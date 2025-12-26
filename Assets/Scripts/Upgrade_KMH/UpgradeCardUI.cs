using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using System.Text;
using TMPro;

public class UpgradeCardUI : MonoBehaviour, IPointerClickHandler //, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] GameObject _selectedEdge;      // 선택 테두리

    [Header("강화 슬롯 체크")]
    [SerializeField] bool _isSlot;

    [Header("카드 UI 요소")]
    [SerializeField] Image _img;                     // 카드 이미지
    [SerializeField] TextMeshProUGUI _nameText;      // 이름 텍스트
    [SerializeField] TextMeshProUGUI _descText;      // 설명 텍스트
    [SerializeField] TextMeshProUGUI _typeText;      // 타입 텍스트
    [SerializeField] TextMeshProUGUI _levelText;     // 레벨 텍스트
    [SerializeField] TextMeshProUGUI _numberOfAvailableText;      // 사용 가능 횟수 텍스트

    [Header("등급 색상")]
    [SerializeField] Image _gradeColorImg;           // 등급 색 이미지
    [SerializeField] Color[] _gradeColors;           // 등급 색

    public UserCard UserCard { get; private set;  } // 보유 카드 데이터

    private CardManager _cardManager;
    private UpgradeManager _upgradeManager;  // 강화 매니저
    private CardData _cardData;              // 카드 데이터


    public void OnPointerClick(PointerEventData eventData)
    {
        if (_isSlot) return;

        _upgradeManager.SelectCard(this);
    }


    // 첫 생성 초기화
    public void Init(UserCard userCard, UpgradeManager manager)
    {
        _cardManager = CardManager.Instance;

        // 보유 카드 정보
        UserCard = userCard;

        // ID 카드 데이터 가져오기
        _cardData = userCard?.GetData();

        // 매니저
        _upgradeManager = manager;

        // 비주얼 설정
        // 강화 슬롯 이면 일단 끔
        if (_isSlot == true) gameObject.SetActive(false);
        else SetVisual();
    }
    
    
    // 카드 데이터에 맞게 비주얼 갱신
    private void SetVisual()
    {
        if (_img != null) _img.sprite = DataManager.Instance.GetCardSprite(_cardData.CardImg);
        else Debug.LogError("Img 가 할당되어있지 않습니다.");
        if (_nameText != null) _nameText.text = _cardData.Name;
        else Debug.LogError("NameText 가 할당되어있지 않습니다.");

        if (_cardData.IsCard == true && _descText != null) _descText.text = _cardData.Desc;
        else if (_descText == null) Debug.LogError("DescText 가 할당되어있지 않습니다.");

        // 설명 갱신
        UpdateUpgradeText();

        // 타입
        SetTypeText();

        // 카드 등급 색상
        SetGradeColor();

        // 선택 테두리
        if(!_isSlot)_selectedEdge.SetActive(false);

    }

    // 카드 등급 색상
    private void SetGradeColor()
    {
        Color color = new Color();

        switch (_cardData.CardGrade)
        {
            case CardGrade.Common:
                color = _gradeColors[0];
                break;
            case CardGrade.Rare:
                color = _gradeColors[1];
                break;
            case CardGrade.Epic:
                color = _gradeColors[2];
                break;
            case CardGrade.Legendary:
                color = _gradeColors[3];
                break;
        }

        _gradeColorImg.color = color;
    }


    // 설명 갱신 (초기, 업그레이드 연출 이후)
    public void UpdateUpgradeText()
    {
        // 카드 레벨 가져오기
        int level = _cardManager.GetCardLevel(_cardData.Id);
        
        // 레벨
        if (_levelText != null) _levelText.text = level.ToString();
        else Debug.LogError("LevelText 가 할당되어있지 않습니다.");

        // 설명
        if (_descText != null) _descText.text = GetDesc(level);
        else if (_descText == null) Debug.LogError("DescText 가 할당되어있지 않습니다.");

        // 카드 사용 가능 횟수
        int numberOfAvailable = _cardManager.GetCardNumberOfAvailable(level, _cardData.CardGrade);
        if (_numberOfAvailableText != null) _numberOfAvailableText.text = numberOfAvailable.ToString("N0");
        else Debug.LogError("NumberOfAvailableText 가 할당되어있지 않습니다.");
    }
    // 타입 설정
    private void SetTypeText()
    {
        if (_typeText != null)
        {
            string type;

            switch (_cardData.CardType)
            {
                case CardType.Attack:
                    type = "공격";
                    break;
                case CardType.Shield:
                    type = "방어";
                    break;
                case CardType.Healing:
                    type = "치유";
                    break;
                case CardType.Spell:
                    type = "주문";
                    break;
                default:
                    type = "NULL";
                    break;
            }
            _typeText.text = type;
        }
        else Debug.LogError("TypeText 가 할당되어있지 않습니다.");
    }

    // 설명 가져오기
    public string GetDesc(int level)
    {
        // 카드 설명 있을 때만
        if (string.IsNullOrEmpty(_cardData.Desc)) return "NULL";

        // 문자열 갱신
        StringBuilder sb = new StringBuilder(_cardData.Desc);

        sb.Replace("{D}", GetValue(level).ToString());
        sb.Replace("{N}", GetValue(level).ToString());
        sb.Replace("{SEV}", GetStatusEffectValue(level).ToString());
        sb.Replace("{turns}", GetTurn(level).ToString());

        return sb.ToString();
    }

    // 카드 수치 반환
    private int GetValue(int level)
    {
        return _cardData.BaseValue + (level - 1) * _cardData.ValuePerValue;
    }
    private int GetStatusEffectValue(int level)
    {
        return _cardData.StatusEffectValue + (level - 1) * _cardData.ValuePerValue;
    }
    private int GetTurn(int level)
    {
        return _cardData.Turn + (level - 1) * _cardData.ValuePerValue;
    }

    // 선택 - 강화 슬롯
    public void Select(UserCard userCard)
    {
        // 데이터 가져와서
        _cardData = userCard?.GetData();

        // UI 변경
        SetVisual();
    }

    // 선택 - 카드 리스트
    public void Select()
    {
        // 선택 테두리
        _selectedEdge?.SetActive(true);
    }


    // 선택 해지 - 카드 리스트
    public void DeSelect()
    {
        // 선택 테두리
        _selectedEdge?.SetActive(false);
    }
}
