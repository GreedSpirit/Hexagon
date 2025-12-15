using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class CardUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] Transform _visual;  // 실제 카드 비주얼
    [SerializeField] Image _edgeColor;   // 테두리 색

    private Card cardLogic;
    private HandManager handManager;    // 핸드 매니저 (카드 제거 시 필요)

    private bool isMouseOver = false;   // 마우스 오버
    private bool isHovering = false;    // 호버 상태
    private bool isDragging = false;    // 드래그 상태
    private bool isReturning = false;   // 복귀 상태

    private Vector3 basePos;            // 부채꼴 위치
    private Quaternion baseRot;         // 부채꼴 회전

    private int siblingIndex;           // 정렬 순서



    private void Update()
    {
        // 드래그 중일 때 무시
        if (isDragging) return;
        // 호버 중일 때 무시
        if (isHovering) return;

        // 원위치
        transform.position = Vector3.Lerp(transform.position, basePos, Time.deltaTime * handManager.MoveSpeed);
        transform.rotation = Quaternion.Lerp(transform.rotation, baseRot, Time.deltaTime * handManager.MoveSpeed);
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, Time.deltaTime * handManager.ScaleSpeed);

        // 카드 비주얼 크기
        // 옵셋만큼 크게
        _visual.localScale = Vector3.Lerp(_visual.localScale, Vector3.one * handManager.HoverOffset, Time.deltaTime * handManager.ScaleSpeed);

        // 복귀 중
        if (isReturning)
        {
            // 원래 위치랑 가까워 지면 
            if (Vector3.Distance(transform.position, basePos) < 1f)
            {
                // 복귀 완료
                isReturning = false;

                // 근데 마우스 올라가 있으면 호버 상태로 전환
                if (isMouseOver)
                    OnHover();
            }
        }
    }


    // ----------------------------------------------------------------

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isDragging) return; // 드래그 중 무시
        isMouseOver = true;     // 마우스 오버
        if (isReturning) return;// 복귀 중 무시

        // 호버 상태 전환
        OnHover();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isDragging) return;  // 드래그 중엔 무시

        // 복귀
        isMouseOver = false;
        isHovering = false;
        isReturning = true;

        // 원래 순서로 복귀
        transform.SetSiblingIndex(siblingIndex);
    }

    // 호버 상태
    private void OnHover()
    {
        // 호버 상태
        isHovering = true;

        // 위로 이동, 회전 0, 확대
        transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.one * handManager.HoverScale;

        // 카드 비주얼 크기
        // 부모와 동일하게
        _visual.localScale = Vector3.one;

        // 옆 카드에 가려지지 않게 맨 앞으로 가져옴
        siblingIndex = transform.GetSiblingIndex();
        transform.SetAsLastSibling();
    }

    // ----------------------------------------------------------------

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 드래그 시작
        isDragging = true;
        isHovering = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 카드가 마우스 위치에 따라옴
        transform.position = eventData.position + Vector2.down * 200f;
        transform.rotation = Quaternion.identity;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 드래그 끝
        isDragging = false;
        isReturning = true;

        // 원래 순서로 복귀
        transform.SetSiblingIndex(siblingIndex);

        // 놓을 때 위치
        // 화면의 UseScreenRatio퍼센트보다 높을 시 사용 시도
        bool isUse = eventData.position.y > Screen.height * handManager.UseScreenRatio;

        // 사용 시도
        if (isUse)
            cardLogic.TryUse();
    }

    // ----------------------------------------------------------------

    // 첫 생성 초기화
    public void Init(HandManager manager)
    {
        cardLogic = GetComponent<Card>();
        handManager = manager;

        // 테스트용 랜덤 색상
        _edgeColor.color = Random.ColorHSV();
    }
    
    // 카드 부채꼴 위치 지정 함수
    public void SetBase(Vector3 pos, Quaternion rot)
    {
        basePos = pos;
        baseRot = rot;
    }

}
