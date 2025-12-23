using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardDetailPanel : MonoBehaviour
{
    [Header("UI 요소")]
    [SerializeField] Image _cardImage;
    [SerializeField] TextMeshProUGUI _nameText;
    [SerializeField] TextMeshProUGUI _typeText;
    [SerializeField] TextMeshProUGUI _descText;
    [SerializeField] TextMeshProUGUI _costText; // 만약 코스트가 있다면
    [SerializeField] GameObject _emptyStateObj; // 선택 안됐을 때 띄울 것

    public void Init()
    {
        // 초기엔 빈 상태
        if (_emptyStateObj != null) _emptyStateObj.SetActive(true);
        gameObject.SetActive(false); // 혹은 패널 자체를 숨김
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
        if (_emptyStateObj != null) _emptyStateObj.SetActive(false);

        CardData data = userCard.GetData();
        if (data == null) return;

        // 텍스트 정보 갱신
        _nameText.text = data.Name;
        _typeText.text = data.CardType.ToString(); // 한글 변환 필요시 StringTable 사용

        // 설명 텍스트 (동적 수치 반영)
        // 지금은 간단하게 data.Desc로 넣지만, 실제로는 수치 파싱 필요
        _descText.text = ParseDescription(data, userCard.Level);

        // 이미지 (리소스 로드 구현 시 주석 해제)
        // _cardImage.sprite = Resources.Load<Sprite>(data.CardImg);
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
        sb.Replace("{Turns}", data.Turn.ToString());

        return sb.ToString();
    }
}