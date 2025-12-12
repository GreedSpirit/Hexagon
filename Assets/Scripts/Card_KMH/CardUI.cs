using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private HandManager handManager;    // 핸드 매니저 (카드 제거 시 필요)

    private bool isHovering = false;    // 호버 상태
    private bool isDragging = false;    // 드래그 상태

    private Vector3 basePos;            // 부채꼴 위치
    private Quaternion baseRot;         // 부채꼴 회전

    private int siblingIndex;           // 정렬 순서

    private void Awake()
    {
        // 테스트용 랜덤 색상
        GetComponent<Image>().color = Random.ColorHSV();
    }


    private void Update()
    {
        // 드래그 중일 땐 무시
        if (isDragging) return;

        // 마우스 오버
        if (isHovering)
        {
            // 보정치만큼 위로
            Vector3 hoverPos = new Vector3(transform.position.x, handManager.BottomOffset, transform.position.z);

            // 위로 이동, 회전 0, 확대
            transform.position = hoverPos;
            transform.rotation = Quaternion.identity;
            transform.localScale = Vector3.one * handManager.HoverScale;
        }
        // 호버, 드래그 둘 다 아닐 때 (기본)
        else
        {
            // 원위치
            transform.position = Vector3.Lerp(transform.position, basePos, Time.deltaTime * handManager.MoveSpeed);
            transform.rotation = Quaternion.Lerp(transform.rotation, baseRot, Time.deltaTime * handManager.MoveSpeed);
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, Time.deltaTime * handManager.ScaleSpeed);
        }
    }


    // ----------------------------------------------------------------

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isDragging) return; // 드래그 중엔 무시

        isHovering = true;

        // 옆 카드에 가려지지 않게 맨 앞으로 가져옴
        siblingIndex = transform.GetSiblingIndex();
        transform.SetAsLastSibling();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isDragging) return;  // 드래그 중엔 무시

        isHovering = false;

        // 원래 순서로 복귀
        transform.SetSiblingIndex(siblingIndex);
    }

    // ----------------------------------------------------------------

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;

        // 드래그 시작하면 호버 상태 해제
        isHovering = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
        transform.rotation = Quaternion.identity;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;

        // 드래그가 끝났을 때 원래 순서로 복귀
        transform.SetSiblingIndex(siblingIndex);

        // 놓을 때 위치
        // 화면의 30퍼센트보다 높을 시 사용
        bool isUse = eventData.position.y > Screen.height * handManager.UseScreenRatio;

        // 사용
        if (isUse)
        {
            handManager.UseCard(this);
        }
    }

    // ----------------------------------------------------------------

    // 첫 생성 초기화
    public void Init(HandManager manager)
    {
        handManager = manager;
    }
    
    // 카드 부채꼴 위치 지정 함수
    public void SetBase(Vector3 pos, Quaternion rot)
    {
        basePos = pos;
        baseRot = rot;
    }

}
