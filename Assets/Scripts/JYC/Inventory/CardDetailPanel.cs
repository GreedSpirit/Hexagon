using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardDetailPanel : MonoBehaviour
{
    [Header("왼쪽: 카드 슬롯")]
    [SerializeField] Image _leftCardImage;    // 카드 일러스트
    [SerializeField] TextMeshProUGUI _leftNameText; // 카드 이름
    [SerializeField] TextMeshProUGUI _leftDescText; // 카드 설명 (본문)
    [SerializeField] TextMeshProUGUI _leftLevelText; // 레벨 숫자
    [SerializeField] TextMeshProUGUI _leftNoaText;   // 사용가능횟수 숫자
    [SerializeField] TextMeshProUGUI _leftTypeText;  // 타입 (공격/방어 등)
    [SerializeField] Image _leftGradeColorImg;       // 등급 색상 네모
    [SerializeField] TextMeshProUGUI _leftCountText; // 보유 개수

    [Header("설정: 등급별 색상")]
    [SerializeField] Color[] _gradeColors; // 0:Common, 1:Rare, 2:Epic, 3:Legendary

    [Header("오른쪽 : UI 요소")]
    [SerializeField] Image _cardImage;
    [SerializeField] TextMeshProUGUI _nameText;
    [SerializeField] TextMeshProUGUI _typeText;
    [SerializeField] TextMeshProUGUI _descText;

    [Header("제어용")]
    [SerializeField] GameObject _contentGroup;    // 카드 정보 그룹 (이미지+텍스트들)
    [SerializeField] GameObject _emptyStateObj; // 선택 안됐을 때 띄울 것

    public void Init()
    {
        // 초기엔 빈 상태
        if (_contentGroup != null) _contentGroup.SetActive(false);
        if (_emptyStateObj != null) _emptyStateObj.SetActive(true);
    }

    // 카드를 선택했을 때 호출
    public void SetCardInfo(UserCard userCard)
    {
        if (userCard == null)
        {
            Init();
            return;
        }
        gameObject.SetActive(true);
        if (_contentGroup != null) _contentGroup.SetActive(true);
        if (_emptyStateObj != null) _emptyStateObj.SetActive(false);

        CardData data = userCard.GetData();
        if (data == null) return;

        Sprite artSprite = DataManager.Instance.GetCardSprite(data.CardImg);
        string parsedDesc = ParseDescription(data, userCard.Level);
        string koreanType = GetKoreanTypeString(data.CardType);
        int numberOfAvailable = CardManager.Instance.GetCardNumberOfAvailable(userCard.Level, data.CardGrade);
        if (_leftCardImage != null) _leftCardImage.sprite = artSprite;
        if (_leftLevelText != null) _leftLevelText.text = userCard.Level.ToString();
        if (_leftCountText != null) _leftCountText.text = $"x {userCard.Count}";
        if (_leftNoaText != null) _leftNoaText.text = numberOfAvailable.ToString();
        if (_leftNameText != null) _leftNameText.text = data.Name;
        if (_leftDescText != null) _leftDescText.text = parsedDesc;
        if (_leftTypeText != null)
        {
            string typeString = "";
            switch (data.CardType)
            {
                case CardType.Attack: typeString = "공격"; break;
                case CardType.Shield: typeString = "방어"; break;
                case CardType.Healing: typeString = "치유"; break;
                case CardType.Spell: typeString = "주문"; break;
                default: typeString = "기타"; break;
            }
            _leftTypeText.text = typeString;
        }
        if (_leftGradeColorImg != null && _gradeColors != null && _gradeColors.Length > 0)
        {
            Color color = Color.white;
            switch (data.CardGrade)
            {
                case CardGrade.Common: if (_gradeColors.Length > 0) color = _gradeColors[0]; break;
                case CardGrade.Rare: if (_gradeColors.Length > 1) color = _gradeColors[1]; break;
                case CardGrade.Epic: if (_gradeColors.Length > 2) color = _gradeColors[2]; break;
                case CardGrade.Legendary: if (_gradeColors.Length > 3) color = _gradeColors[3]; break;
            }
            _leftGradeColorImg.color = color;
        }

        // 텍스트 설정
        if (_nameText != null) _nameText.text = data.Name;
        if (_typeText != null) _typeText.text = koreanType;
        if (_descText != null) _descText.text = ParseDescription(data, userCard.Level);

        //  이미지 설정
        if (_cardImage != null)
        {
            _cardImage.sprite = DataManager.Instance.GetCardSprite(data.CardImg);
        }
    }

    // 설명에 {D}, {N} 같은 태그 값을 실제 수치로 변환
    private string ParseDescription(CardData data, int level)
    {
        if (string.IsNullOrEmpty(data.Desc)) return "";

        // 기본값 + (레벨-1 * 증가량)
        int finalValue = data.BaseValue + (level - 1) * data.ValuePerValue;

        System.Text.StringBuilder sb = new System.Text.StringBuilder(data.Desc);
        sb.Replace("{D}", finalValue.ToString());
        sb.Replace("{N}", finalValue.ToString());
        sb.Replace("{SEV}", data.StatusEffectValue.ToString());
        sb.Replace("{turns}", data.Turn.ToString());

        return sb.ToString();
    }
    private string GetKoreanTypeString(CardType type)
    {
        switch (type)
        {
            case CardType.Attack: return "공격";
            case CardType.Shield: return "방어";
            case CardType.Healing: return "치유";
            case CardType.Spell: return "주문";
            default: return "기타";
        }
    }
}