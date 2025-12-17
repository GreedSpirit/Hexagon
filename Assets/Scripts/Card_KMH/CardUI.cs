using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class CardUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] Transform _visual;             // 실제 카드 비주얼 트랜스폼
    [SerializeField] GameObject _selectedEdge;      // 선택 테두리

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
    private CanvasGroup _canvasGroup;    // 오버드로우 소멸 테스트용

    private bool _isMouseOver = false;   // 마우스 오버
    private bool _isHovering = false;    // 호버 상태
    private bool _isDragging = false;    // 드래그 상태
    private bool _isReturning = false;   // 복귀 상태
    private bool _isSelected = false;    // 선택 상태
    private bool _isOverDrawing = false; // 오버드로우 상태

    private Vector3 _basePos;            // 부채꼴 위치
    private Quaternion _baseRot;         // 부채꼴 회전

    private Vector3 dragPos;             // 드래그 좌표

    private int _siblingIndex;           // 정렬 순서


    private void Update()
    {
        // 오버드로우 연출 중일 때
        if (_isOverDrawing)
        {
            KeepCardOnTop();
            return;
        }

        // 테스트용
        // 마우스 우 클릭 뗄 때
        if (Mouse.current.rightButton.wasReleasedThisFrame)
        {
            // 카드 선택 강제 취소
            Cancel();
            return;
        }

        // 드래그 중일 때
        if (_isDragging)
        {
            // 항상 맨 앞으로
            KeepCardOnTop();
            // 카드가 드래그 위치 따라가게
            transform.position = Vector3.Lerp(transform.position, dragPos, Time.deltaTime * _handManager.MoveSpeed);
            return;
        }
        // 호버 중일 때 무시
        if (_isHovering)
        {
            // 항상 맨 앞으로
            KeepCardOnTop();
            return;
        }

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

                // 마우스 올라가 있으면 호버 상태로 전환
                // 근데 선택된 카드 없어야 함
                if (_isMouseOver &&_handManager.SelectedCard == null)
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
        if (_handManager.SelectedCard != null) return;  // 선택된 카드 있으면 무시

        // 호버 상태 전환
        OnHover();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_isDragging) return;  // 드래그 중엔 무시

        _isMouseOver = false;     // 마우스 오버 상태 끝

        if (_isSelected) return;  // 선택 된 상태면 무시
         
        if (_isHovering)          // 호버 상태 라면
        {
            // 복귀
            _isHovering = false;
            _isReturning = true;

            // 원래 순서로 복귀
            transform.SetSiblingIndex(_siblingIndex);
        }
    }

    public void OnPointerDown(PointerEventData eventData) { }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (_isReturning) return;// 복귀 중 무시
        if (_isDragging) return; // 드래그로 인한 클릭 방지

        // 선택된 상태라면 사용 시도
        if (_isSelected)
        {
            // 카드 사용 시도
            _cardLogic.TryUse();

            // 사용 시도 후 선택 해제
            _handManager.DeselectCard();
        }
        else // 선택된 상태가 아니라면 선택
        {
            _isSelected = true;
            // 크게
            OnHover();
            // 선택 테두리
            _selectedEdge.SetActive(true);
        }
    }

    // ----------------------------------------------------------------

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 드래그 시작
        // 선택된 카드로 등록
        _handManager.SetSelectedCard(this);
        _isSelected = true;                  // 드래그 중에도 선택 상태
        _isDragging = true;

        // 드래그 시 즉시 호버 (다른 카드 선택 중일 때 대비)
        OnHover();
        _isHovering = false;

    }

    public void OnDrag(PointerEventData eventData)
    {
        // 드래그 위치 설정
        dragPos = eventData.position;
        transform.rotation = Quaternion.identity;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 취소로 인한 예외 방지
        if (_isDragging == false) return;

        // 드래그 끝
        _isDragging = false;
        _isReturning = true;

        // 원래 순서로 복귀
        transform.SetSiblingIndex(_siblingIndex);

        // 놓을 때 위치
        // 화면의 UseScreenRatio퍼센트보다 높을 시 사용 시도
        bool isUse = eventData.position.y > Screen.height * _handManager.UseScreenRatio;

        if (isUse)
        {
            // 사용 시도
            _cardLogic.TryUse();
        }

        // 선택 해제
        _handManager.DeselectCard();
    }

    // ----------------------------------------------------------------

    // 첫 생성 초기화
    public void Init(CardData data, HandManager manager)
    {
        _cardLogic = GetComponent<CardLogic>();
        _canvasGroup = GetComponent<CanvasGroup>();

        _cardData = data;
        _handManager = manager;

        _isSelected = false;
        _isReturning = false;
        _isHovering = false;
        _isMouseOver = false;
        _isOverDrawing = false;

        // 비주얼 설정
        SetVisual();
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

        // 선택 테두리 기본 off
        _selectedEdge.SetActive(false);

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

    // 카드 선택 해지
    public void Deselect()
    {
        _isSelected = false;

        // 복귀
        transform.SetSiblingIndex(_siblingIndex);

        OnPointerExit(null);
    }

    // 카드 선택, 드래그 취소 (우클릭)
    private void Cancel()
    {
        // 드래그 상태 강제 해제
        _isDragging = false;

        // 선택 상태 해제
        _isSelected = false;

        // 핸드 매니저 카드 해지
        _handManager.DeselectCard();
    }

    // 호버 상태 (클릭 선택 상태)
    private void OnHover()
    {
        // 호버 상태
        _isHovering = true;

        // 바닥 띄우기 (핸드 매니저 y + 카드 높이 절반 * 호버 배율)
        float yOffset = _handManager.transform.position.y + _handManager.CardHalfHeight * _handManager.HoverScale;

        // 위로 이동, 회전 0, 확대
        if (_isOverDrawing == false)     // 오버드로우 때는 위치 이동 방지
            transform.position = new Vector3(transform.position.x, yOffset, transform.position.z);
        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.one * _handManager.HoverScale;

        // 카드 비주얼 크기
        // 부모와 동일하게
        _visual.localScale = Vector3.one;
    }

    // 카드 맨 위 고정
    private void KeepCardOnTop()
    {
        // 카드 수
        int lastIndex = transform.parent.childCount - 1;

        // 내 현재 순서가 마지막이 아니라면
        if (transform.GetSiblingIndex() < lastIndex)
        {
            // 마지막으로 변경
            transform.SetAsLastSibling();
        }
    }

    // 오버 드로우 카드면 호출
    public void OnOverDraw(Transform targetPos)
    {
        // 마우스 입력 차단
        _canvasGroup.blocksRaycasts = false;

        // Update 차단
        _isOverDrawing = true;

        // 크게
        OnHover();             

        // 이동, 소멸 시작
        StartCoroutine(OverDraw(targetPos));
    }

    // 오버 드로우 카드면 파괴
    private IEnumerator OverDraw(Transform overDrawPoint)
    {
        // 이동 (카드와 오버드로우 포인트 거리 체크)
        while (Vector3.Distance(transform.position, overDrawPoint.position) > 0.1f)
        {
            // 카드 위치를 오버드로우 포인트까지 이동
            transform.position = Vector3.Lerp(transform.position, overDrawPoint.position, Time.deltaTime * _handManager.MoveSpeed);

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
