using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeScrollBar : MonoBehaviour
{
    [Header("스크롤바")]
    [SerializeField] Scrollbar _targetScroll; // 스크롤바

    [Header("설정")]
    [SerializeField] float waitTime = 3.0f;       // 대기 시간

    private CanvasGroup _cg;
    private float _lastScrollTime;      // 마지막으로 스크롤한 시간

    private void Awake()
    {
        _cg = _targetScroll.GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        // 첨엔 안보임
        _cg.alpha = 0f;

        // OnScroll 함수 연결
        _targetScroll.onValueChanged.AddListener(OnScroll);
    }
    private void Update()
    {
        // 이미 투명하면 무시
        if (_cg.alpha <= 0f) return;

        // 마지막 스크롤 시간에서 3초가 지났는지 체크
        if (Time.time > _lastScrollTime + waitTime)
        {
            // 3초 지나면 투명
            _cg.alpha = 0f;
        }
    }
    private void OnDestroy()
    {
        // 혹시 모르니 연결 해제
        _targetScroll?.onValueChanged.RemoveListener(OnScroll);
    }

    // 스크롤 변화 감지
    private void OnScroll(float value)
    {
        // 보이게
        _cg.alpha = 1f;

        // 마지막 스크롤 시간 갱신
        _lastScrollTime = Time.time;
    }
}
