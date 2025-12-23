using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class BattleDeckUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("덱 카운트")]
    [SerializeField] GameObject _countPanel;
    [SerializeField] TextMeshProUGUI _countText;

    [Header("포커스 테두리")]
    [SerializeField] GameObject _edge;

    private HandManager _handManager;

    void Start()
    {
        ChangeActive();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ChangeActive();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ChangeActive();
    }

    // 포커스 전환
    private void ChangeActive()
    {
        _edge.SetActive(!_edge.activeSelf);
        _countPanel.SetActive(!_countPanel.activeSelf);
    }

    // 덱 남은 카드 수 텍스트 갱신
    public void UpdateDeckCountText()
    {
        _countText.text = _handManager.CurrentDeckCount.ToString();
    }

    public void Init(HandManager handManager)
    {
        _handManager = handManager;
    }
}
