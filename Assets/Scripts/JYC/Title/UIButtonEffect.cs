using UnityEngine;
using UnityEngine.EventSystems; // 필수
using System.Collections;

public class UIButtonEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("설정")]
    [SerializeField] private float _scaleSize = 1.1f; // 1.1배
    [SerializeField] private float _duration = 0.1f;  // 변하는 시간 (초)

    private Vector3 _originalScale;
    private Coroutine _scaleCoroutine;

    private void Awake()
    {
        _originalScale = transform.localScale;
    }

    // 마우스가 버튼 위에 올라갔을 때
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_scaleCoroutine != null) StopCoroutine(_scaleCoroutine);
        _scaleCoroutine = StartCoroutine(ScaleTo(_originalScale * _scaleSize));

    }

    // 마우스가 버튼에서 나갔을 때
    public void OnPointerExit(PointerEventData eventData)
    {
        if (_scaleCoroutine != null) StopCoroutine(_scaleCoroutine);
        _scaleCoroutine = StartCoroutine(ScaleTo(_originalScale));
    }

    // 크기 변화 코루틴
    private IEnumerator ScaleTo(Vector3 targetScale)
    {
        float timer = 0f;
        Vector3 startScale = transform.localScale;

        while (timer < _duration)
        {
            timer += Time.unscaledDeltaTime; // 타임스케일 무시 (일시정지여도 작동하게)
            transform.localScale = Vector3.Lerp(startScale, targetScale, timer / _duration);
            yield return null;
        }
        transform.localScale = targetScale;
    }

}