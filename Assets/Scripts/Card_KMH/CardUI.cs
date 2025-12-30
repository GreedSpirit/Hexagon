using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

enum CardUIState
{
    Idle,       // 기본
    Hover,      // 마우스 오버
    Selected,   // 선택
    Drag,       // 드래그
    Return,     // 복귀
    Disappear   // 소멸
}

public class CardUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] Transform _visual;             // 실제 카드 비주얼 트랜스폼
    [SerializeField] GameObject _selectedEdge;      // 선택 테두리

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

    // HandManager에서 우클릭, 휠클릭 등으로 선택 취소 시 사용
    public bool IsDragging => _currentState == CardUIState.Drag;


    private CardLogic _cardLogic;        // 카드 로직
    private CardTooltipLogic _tooltipLogic;// 툴팁
    private CardData _cardData;          // 카드 데이터
    private HandManager _handManager;    // 핸드 매니저 (카드 제거 시 필요)
    private CanvasGroup _canvasGroup;    // 소멸 테스트용

    // 카드 상태
    private CardUIState _currentState = CardUIState.Idle;

    private Vector3 _basePos;            // 부채꼴 위치
    private Quaternion _baseRot;         // 부채꼴 회전

    private Vector3 _dragPos;             // 드래그 좌표

    private int _siblingIndex;           // 정렬 순서


    private void Update()
    {
        // 상태별 위치 이동 로직
        switch (_currentState)
        {
            case CardUIState.Idle:
                ReturnToIdlePosition();// 기본 위치로 이동
                break;

            case CardUIState.Hover:
            case CardUIState.Selected:
                MoveToHoverPosition(); // 호버, 선택 위치 이동
                break;

            case CardUIState.Drag:
                MoveToDragPosition();  // 드래그 위치 이동
                break;

            case CardUIState.Return:
                ReturnToIdlePosition();// 기본 위치로 이동
                CheckReturnDistance(); // 복귀 체크
                break;
        }
    }


    // 상태 전환
    private void ChangeState(CardUIState newState)
    {
        if (_currentState == newState) return; // 같은 상태면 무시

        // 상태 나갈 때
        switch (_currentState)
        {
            case CardUIState.Selected:          // 선택, 드래그 상태
            case CardUIState.Hover:
                _selectedEdge.SetActive(false); // 테두리 끄기
                _tooltipLogic.PointerExitParent();   // 툴팁 비활성화
                break;
            case CardUIState.Drag:
                _selectedEdge.SetActive(false); // 테두리 끄기
                break;
        }

        _currentState = newState;

        // 상태 전환될 때
        switch (_currentState)
        {
            case CardUIState.Idle:
            case CardUIState.Return:
                transform.SetSiblingIndex(_siblingIndex); // 순서 복구
                break;

            case CardUIState.Hover:
                transform.SetAsLastSibling(); // 맨 위로
                _selectedEdge.SetActive(true);// 테두리 활성화
                _tooltipLogic.PointerEnterParent(); // 툴팁 활성화
                break;
            case CardUIState.Selected:
                transform.SetAsLastSibling();
                _handManager.SetSelectedCard(this); // 선택 카드 등록
                _selectedEdge.SetActive(true);
                _tooltipLogic.PointerEnterParent();    // 툴팁 활성화
                break;
            case CardUIState.Drag:
                transform.SetAsLastSibling();
                _handManager.SetSelectedCard(this); // 선택 카드 등록
                _selectedEdge.SetActive(true);
                break;
            case CardUIState.Disappear:
                transform.SetParent(_handManager.HandTransform.parent, true);   // 바깥으로 끄집어내기 (절대 가려지지 않게)
                transform.SetAsLastSibling();
                _canvasGroup.blocksRaycasts = false;                        // 마우스 처리 안되게
                // 크기를 호버 크기로
                transform.localScale = Vector3.one * _handManager.HoverScale;
                if (_visual != null) _visual.localScale = Vector3.one;
                break;
        }
    }

    // ----------------------------------------------------------------

    public void OnPointerEnter(PointerEventData eventData)
    {
        // 기본 상태일 때
        if (_currentState != CardUIState.Idle) return;

        // 다른 카드 선택되어 있으면 무시
        if (_handManager.SelectedCard != null) return;

        // 호버 상태로 전환
        ChangeState(CardUIState.Hover);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // 호버 상태일 때
        // 선택 상태면 마우스 나가도 유지
        if (_currentState == CardUIState.Hover)
        {
            // 기본 상태로 전환
            ChangeState(CardUIState.Idle);
        }
    }

    public void OnPointerDown(PointerEventData eventData) { }

    public void OnPointerUp(PointerEventData eventData)
    {
        // 좌클릭 릴리즈 아니면 무시
        if (eventData.button != PointerEventData.InputButton.Left) return;

        // 드래그, 복귀 상태는 무시
        if (_currentState == CardUIState.Drag || _currentState == CardUIState.Return) return;

        // 다른 카드 선택되어 있을 경우
        if (_handManager.SelectedCard != null && _handManager.SelectedCard != this)
        {
            // 기존 선택 카드 해제
            _handManager.SelectedCard.Deselect();

            // 선택 상태로 전환
            ChangeState(CardUIState.Selected);
            return;
        }

        // 선택 상태라면
        if (_currentState == CardUIState.Selected)
        {
            // 2 번째 선택 클릭은 사용 시도
            _cardLogic.TryUse();

            // 카드 선택 해제
            _handManager.DeselectCard();
        }
        else
        {
            // 선택 상태로 전환
            ChangeState(CardUIState.Selected);
        }
    }

    // ----------------------------------------------------------------

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 좌클릭 드래그 아니면 무시
        if (eventData.button != PointerEventData.InputButton.Left) return;

        // 다른 카드가 선택된 상태 드래그 하면
        if (_handManager.SelectedCard != null && _handManager.SelectedCard != this)
        {
            _handManager.SelectedCard.Deselect(); // 기존 카드 해지
        }

        // 드래그 상태로 전환
        ChangeState(CardUIState.Drag);

    }

    public void OnDrag(PointerEventData eventData)
    {
        // 드래그 위치 설정
        if (_currentState == CardUIState.Drag)
        {
            _dragPos = eventData.position;
            transform.rotation = Quaternion.identity;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 드래그 풀리고 호출 시 무시
        if (_currentState != CardUIState.Drag) return;

        Debug.Log("드래그 끝");

        // 좌클릭 드래그 중 우클릭, 휠클릭 등으로 호출될 시 무시
        if (Mouse.current.leftButton.wasReleasedThisFrame == false)
        {
            // 그냥 복귀 상태로
            _handManager.DeselectCard();
            ChangeState(CardUIState.Return);
            return;
        }

        // 사용 범위 체크
        bool isUse = eventData.position.y > Screen.height * _handManager.UseScreenRatio;

        if (isUse)
        {
            // 사용 시도
            _cardLogic.TryUse();
        }
        else
        {
            // 사용 안 하면 복귀 상태
            ChangeState(CardUIState.Return);
        }

        // 선택 카드 해지
        _handManager.DeselectCard();
    }

    // ----------------------------------------------------------------

    // 첫 생성 초기화
    public void Init(CardData data, HandManager manager)
    {
        _cardLogic = GetComponent<CardLogic>();
        _tooltipLogic = GetComponent<CardTooltipLogic>();
        _canvasGroup = GetComponent<CanvasGroup>();

        _cardData = data;
        _handManager = manager;

        // 비주얼 설정
        SetVisual();

        // 상태 설정
        ChangeState(CardUIState.Idle);
    }

    // 카드 부채꼴 위치 지정 함수
    public void SetBase(Vector3 pos, Quaternion rot, int index)
    {
        _basePos = pos;
        _baseRot = rot;

        _siblingIndex = index;
    }

    // 카드 데이터에 맞게 비주얼 갱신
    private void SetVisual()
    {
        if (_img != null) _img.sprite = DataManager.Instance.GetCardSprite(_cardData.CardImg);
        else Debug.LogError("Img 가 할당되어있지 않습니다.");
        if (_nameText != null) _nameText.text = _cardData.Name;
        else Debug.LogError("NameText 가 할당되어있지 않습니다.");
        if (_levelText != null) _levelText.text = _cardLogic.Level.ToString();
        else Debug.LogError("LevelText 가 할당되어있지 않습니다.");

        if (_cardData.IsCard == true && _descText != null) _descText.text = _cardData.Desc;
        else if (_descText == null) Debug.LogError("DescText 가 할당되어있지 않습니다.");

        // 설명 설정
        SetDescText();

        // 타입 설정
        SetTypeText();

        // 카드 등급 색상
        SetGradeColor();

        // 선택 테두리
        _selectedEdge.SetActive(false);
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

    // 설명 설정
    public void SetDescText()
    {
        if (_descText != null)
            _descText.text = _cardLogic.Desc;
        
        else if (_descText == null) Debug.LogError("DescText 가 할당되어있지 않습니다.");
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

    // 기본 상태 위치로 이동
    private void ReturnToIdlePosition()
    {
        // 카드
        transform.position = Vector3.Lerp(transform.position, _basePos, Time.deltaTime * _handManager.MoveSpeed);
        transform.rotation = Quaternion.Lerp(transform.rotation, _baseRot, Time.deltaTime * _handManager.MoveSpeed);
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, Time.deltaTime * _handManager.ScaleSpeed);

        // 카드 비주얼
        if (_visual != null)
            _visual.localScale = Vector3.Lerp(_visual.localScale, Vector3.one * _handManager.HoverVisualScaleOffset, Time.deltaTime * _handManager.ScaleSpeed);
    }

    // 호버 상태 위치로 이동
    private void MoveToHoverPosition()
    {
        // 바닥 띄우기 (핸드 매니저 y + 카드 높이 절반 * 호버 배율)
        float yOffset = _handManager.transform.position.y + _handManager.CardHalfHeight * _handManager.HoverScale;

        // 호버 시 부드러운 이동 원하면 주석 사용하기
        //Vector3 targetPos = new Vector3(transform.position.x, yOffset, transform.position.z);
        //transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * _handManager.MoveSpeed);

        // X축 유지하고 Y축만 띄움
        transform.position = new Vector3(transform.position.x, yOffset, transform.position.z);
        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.one * _handManager.HoverScale;
        
        // 비주얼 크기
        // 부모와 동일하게
        if(_visual != null)
             _visual.localScale = Vector3.one;
    }

    // 드래그 상태 위치로 이동
    private void MoveToDragPosition()
    {
        // 우클릭 취소 체크 ( HandManager가 체크 못 한 경우 )
        if (Mouse.current.rightButton.wasReleasedThisFrame)
        {
            _handManager.DeselectCard();     // 선택 카드 해지
            ChangeState(CardUIState.Return); // 강제 복귀
            return;
        }
        transform.position = Vector3.Lerp(transform.position, _dragPos, Time.deltaTime * _handManager.MoveSpeed);
        transform.localScale = Vector3.one * _handManager.HoverScale;

        // 비주얼 크기
        // 부모와 동일하게
        if (_visual != null)
            _visual.localScale = Vector3.one;
    }

    // 복귀 상태 위치로 이동
    private void CheckReturnDistance()
    {
        // 거의 도착했으면
        if (Vector3.Distance(transform.position, _basePos) < 0.1f)
        {
            // 기본 상태로 전환
            ChangeState(CardUIState.Idle);
        }
    }

    // 카드 선택 해지
    public void Deselect()
    {
        // 선택, 드래그 상태일 때
        if (_currentState == CardUIState.Selected || _currentState == CardUIState.Drag)
        {
            Debug.Log("해지");
            // 복귀 상태로 전환
            ChangeState(CardUIState.Return);
        }
    }

    // 카드 소멸 호출
    public void OnDisappear(Transform disappearPoint)
    {
        // 소멸 상태로 전환
        ChangeState(CardUIState.Disappear);

        // 매개변수 필요해서 여기서 시작
        // 이동, 소멸 시작
        StartCoroutine(CardDisappear(disappearPoint));
    }

    // 소멸
    private IEnumerator CardDisappear(Transform disappearPoint)
    {
        // 이동 (카드와 소멸 포인트 거리 체크)
        while (Vector3.Distance(transform.position, disappearPoint.position) > 0.1f)
        {
            // 카드 위치를 소멸 포인트까지 이동
            transform.position = Vector3.Lerp(transform.position, disappearPoint.position, Time.deltaTime * _handManager.MoveSpeed);

            yield return null;
        }

        float fadeTime = 0.5f; // 사라질 시간
        float timer = 0f;

        // 소멸
        while (timer < fadeTime)
        {
            // 시간 증가
            timer += Time.deltaTime;

            // 남은 시간 비율로 알파값 1 에서 0으로
            _canvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / fadeTime);

            yield return null;
        }

        // 삭제 (나중엔 반환)
        Destroy(gameObject);
    }
}