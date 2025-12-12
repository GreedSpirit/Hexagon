using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    [Header("카드 프리팹")]
    [SerializeField] CardUI cardPrefab;     // 카드 프리팹

    [Header("카드 설정값")]
    [SerializeField] float _moveSpeed = 15f;        // 이동/회전 속도
    [SerializeField] float _scaleSpeed = 15f;       // 확대/축소 속도
    [SerializeField] float _hoverScale = 1.5f;      // 마우스 올렸을 때 커지는 배율
    [SerializeField] float _useScreenRatio = 0.3f;  // 카드 사용할 화면 높이 비율

    [Header("부채꼴 설정")]
    [SerializeField] float maxAngle = 100f;    // 부채꼴 최대 각도
    [SerializeField] float maxSpacing = 10f;   // 카드 간 최대 간격
    [SerializeField] float radius = 1000f;     // 부채꼴 반지름 (클수록 완만)

    public float MoveSpeed => _moveSpeed;
    public float ScaleSpeed => _scaleSpeed;
    public float HoverScale => _hoverScale;
    public float UseScreenRatio => _useScreenRatio;


    // 현재 들고 있는 카드 리스트
    private List<CardUI> myCards = new List<CardUI>();



    // 테스트용 드로우 버튼에 연결
    public void DrawCard()
    {
        // 카드 생성
        CardUI card = Instantiate(cardPrefab, transform.position, Quaternion.identity, transform);

        // 리스트 추가
        myCards.Add(card);

        // 매니저 연결
        card.Init(this);

        // 정렬
        AlignCards();
    }

    // 카드 사용
    public void UseCard(CardUI card)
    {
        // 리스트에서 제외
        myCards.Remove(card);
        // 삭제
        Destroy(card.gameObject);
        // 남은 카드 재정렬
        AlignCards();           
    }

    // 부채꼴 계산
    void AlignCards()
    {
        // 카드 수
        int count = myCards.Count;

        // 부채꼴 범위 정하기 (카드가 적으면 좁게, 많으면 maxAngle까지)
        float currentAngle = Mathf.Min((count - 1) * maxSpacing, maxAngle);

        // 카드 사이의 간격
        float currentAngleStep = count > 1 ? currentAngle / (count - 1) : 0;

        // 시작 각도 설정
        // 전체 각도의 절반만큼 왼쪽으로
        float startAngle = currentAngle / 2f;

        // 중심 포인트 지정 (반지름 만큼 아래로)
        Vector3 center = transform.position - (Vector3.up * radius);


        for (int i = 0; i < count; i++)
        {
            // 현재 카드 각도
            float angleDeg = startAngle - (currentAngleStep * i);

            // 회전
            Quaternion rotation = Quaternion.Euler(0, 0, angleDeg);

            // 위치
            Vector3 direction = rotation * Vector3.up;
            Vector3 position = center + (direction * radius);

            // 카드 위치 설정
            myCards[i].SetBase(position, rotation);

            // 오른쪽이 맨 위로 올라오게
            myCards[i].transform.SetSiblingIndex(i);
        }
    }
}
