using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class CardUI : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] Transform _visual;             // 실제 카드 비주얼 트랜스폼

    [SerializeField] Image _img;                     // 카드 이미지
    [SerializeField] TextMeshProUGUI _nameText;      // 이름 텍스트
    [SerializeField] TextMeshProUGUI _descText;      // 설명 텍스트
    [SerializeField] TextMeshProUGUI _typeText;      // 타입 텍스트
    [SerializeField] TextMeshProUGUI _levelText;     // 레벨 텍스트
    [SerializeField] TextMeshProUGUI _numberOfAvailableText;      // 사용 가능 횟수 텍스트
    [SerializeField] Image _edgeColor;              // 등급 색

    private CardLogic _cardLogic;             // 카드 로직
    private CardData _cardData;          // 카드 데이터
    private HandManager _handManager;    // 핸드 매니저 (카드 제거 시 필요)

    private bool _isMouseOver = false;   // 마우스 오버
    private bool _isHovering = false;    // 호버 상태
    private bool _isDragging = false;    // 드래그 상태
    private bool _isReturning = false;   // 복귀 상태
    private bool _isSelected = false;    // 선택 상태

    private Vector3 _basePos;            // 부채꼴 위치
    private Quaternion _baseRot;         // 부채꼴 회전

    private int _siblingIndex;           // 정렬 순서



    private void Update()
    {
        // 드래그 중일 때 무시
        if (_isDragging) return;
        // 호버 중일 때 무시
        if (_isHovering) return;

        // 원위치
        transform.position = Vector3.Lerp(transform.position, _basePos, Time.deltaTime * _handManager.MoveSpeed);
        transform.rotation = Quaternion.Lerp(transform.rotation, _baseRot, Time.deltaTime * _handManager.MoveSpeed);
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, Time.deltaTime * _handManager.ScaleSpeed);

        // 카드 비주얼 크기
        // 옵셋만큼 크게
        _visual.localScale = Vector3.Lerp(_visual.localScale, Vector3.one * _handManager.HoverVisualScaleOffset, Time.deltaTime * _handManager.ScaleSpeed);

        // 복귀 중
        if (_isReturning)
        {
            // 원래 위치랑 가까워 지면 
            if (Vector3.Distance(transform.position, _basePos) < 1f)
            {
                // 복귀 완료
                _isReturning = false;

                // 근데 마우스 올라가 있으면 호버 상태로 전환
                if (_isMouseOver)
                    OnHover();
            }
        }
    }


    // ----------------------------------------------------------------

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_isDragging) return; // 드래그 중 무시
        _isMouseOver = true;     // 마우스 오버
        if (_isReturning) return;// 복귀 중 무시

        // 호버 상태 전환
        OnHover();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_isReturning) return;// 복귀 중 무시

        // 선택된 상태라면 사용 시도
        if (_isSelected)
        {
            _cardLogic.TryUse();

            // 일단 사용 시도 후 선택 해제
            // _handManager.DeSelect();
        }
        // 선택된 상태가 아니라면 선택
        else
        {
            _isSelected = true;
            //_handManager.SelectCard(this);
        }

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_isDragging) return;  // 드래그 중엔 무시

        // 복귀
        _isMouseOver = false;
        _isHovering = false;
        _isReturning = true;

        // 원래 순서로 복귀
        transform.SetSiblingIndex(_siblingIndex);
    }

    // 호버 상태
    private void OnHover()
    {
        // 호버 상태
        _isHovering = true;

        // 바닥 띄우기 (핸드 매니저 y + 카드 높이 절반 * 호버 배율)
        float yOffset = _handManager.transform.position.y + _handManager.CardHalfHeight * _handManager.HoverScale;

        // 위로 이동, 회전 0, 확대
        transform.position = new Vector3(transform.position.x, yOffset, transform.position.z);
        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.one * _handManager.HoverScale;

        // 카드 비주얼 크기
        // 부모와 동일하게
        _visual.localScale = Vector3.one;

        // 옆 카드에 가려지지 않게 맨 앞으로 가져옴
        _siblingIndex = transform.GetSiblingIndex();
        transform.SetAsLastSibling();
    }

    // ----------------------------------------------------------------

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 드래그 시작
        _isDragging = true;
        _isHovering = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 카드가 마우스 위치에 따라옴
        transform.position = eventData.position;
        transform.rotation = Quaternion.identity;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 드래그 끝
        _isDragging = false;
        _isReturning = true;

        // 원래 순서로 복귀
        transform.SetSiblingIndex(_siblingIndex);

        // 놓을 때 위치
        // 화면의 UseScreenRatio퍼센트보다 높을 시 사용 시도
        bool isUse = eventData.position.y > Screen.height * _handManager.UseScreenRatio;

        // 사용 시도
        if (isUse)
            _cardLogic.TryUse();
    }

    // ----------------------------------------------------------------

    // 첫 생성 초기화
    public void Init(CardData data, HandManager manager)
    {
        _cardLogic = GetComponent<CardLogic>();
        _cardData = data;
        _handManager = manager;

        // 비주얼 설정
        SetVisual();
    }
    
    // 카드 부채꼴 위치 지정 함수
    public void SetBase(Vector3 pos, Quaternion rot)
    {
        _basePos = pos;
        _baseRot = rot;
    }

    // 카드 데이터에 맞게 비주얼 갱신
    private void SetVisual()
    {
        // img = (data.CardImg);
        if (_nameText != null) _nameText.text = _cardData.Name;
        else Debug.LogError("NameText 가 할당되어있지 않습니다.");
        if (_typeText != null) _typeText.text = _cardData.CardType.ToString();
        else Debug.LogError("TypeText 가 할당되어있지 않습니다.");
        if (_levelText != null) _levelText.text = _cardData.Level.ToString();
        else Debug.LogError("LevelText 가 할당되어있지 않습니다.");

        if (_cardData.IsCard == true &&_descText != null) _descText.text = _cardData.GetCardDescWithValue();
        else if(_descText == null) Debug.LogError("DescText 가 할당되어있지 않습니다.");

        // 카드 사용 가능 횟수 (덱 구성, 인벤토리에서만 가능하게)
        int numberOfAvailable = TestGameManager_KMH.Instance.GetCardNumberOfAvailable(_cardData.Level, _cardData.CardGrade);
        if (_numberOfAvailableText != null) _numberOfAvailableText.text = numberOfAvailable.ToString("N0");
        else Debug.LogError("NumberOfAvailableText 가 할당되어있지 않습니다.");

        // 카드 등급 색상
        SetGradeColor();
    }

    // 카드 등급 색상
    private void SetGradeColor()
    {
        Color color = new Color();

        switch (_cardData.CardGrade)
        {
            case CardGrade.Common:
                color = Color.lightGray;
                break;
            case CardGrade.Rare:
                color = Color.cyan;
                break;
            case CardGrade.Epic:
                color = Color.purple;
                break;
            case CardGrade.Legendary:
                color = Color.orange;
                break;
        }

        _edgeColor.color = color;
    }
}
