using UnityEngine;
using System.Collections;

public class MonsterDeathEffect : MonoBehaviour
{
    [Header("설정")]
    [Tooltip("사라지는 데 걸리는 시간")]
    public float fadeDuration = 1.5f;

    [Tooltip("아래로 내려가는 거리 (0이면 제자리에서 투명해짐)")]
    public float sinkDistance = 0.5f;

    private SpriteRenderer spriteRenderer;
    private bool isDead = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        StartCoroutine(FadeAndSink());
    }

    IEnumerator FadeAndSink()
    {
        float elapsedTime = 0f;
        Vector3 startPos = transform.position;
        // Y축으로 sinkDistance만큼 아래로 목표 설정
        Vector3 endPos = startPos + Vector3.down * sinkDistance; 

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float percent = elapsedTime / fadeDuration;

            // 투명도 조절 (Alpha 1 -> 0)
            if (spriteRenderer != null)
            {
                Color color = spriteRenderer.color;
                color.a = Mathf.Lerp(1f, 0f, percent);
                spriteRenderer.color = color;
            }

            // 위치 이동 (아래로 서서히 이동)
            transform.position = Vector3.Lerp(startPos, endPos, percent);

            yield return null;
        }

        // 완전히 사라지면 삭제
        Destroy(gameObject);
    }
}