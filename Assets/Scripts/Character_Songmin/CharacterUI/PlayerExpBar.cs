using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerExpBar : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Slider _expBar;
    [SerializeField] private GameObject _expText;

    private void Awake()
    {
        if (_expBar == null)
        {
            _expBar = GetComponent<Slider>();
        }
    }

    public void OnEnable()
    {
        if (Player.Instance != null)
        {
            Player.Instance.OnExpChanged += UpdateExpBar;
            Player.Instance.PushExp();
        }
        else
        {
            Debug.LogError("Player.Instance가 생성되지 않았습니다.");
        }

    }

    public void UpdateExpBar(int currentExp, int needExp)
    {
        if (currentExp != 0)
        {
            _expBar.value = (float)currentExp / needExp;
        }
        else
        {
            _expBar.value = 0;
        }

    }

    public void OnDisable()
    {
        if (Player.Instance != null)
        {
            Player.Instance.OnExpChanged -= UpdateExpBar;
        }
        else
        {
            Debug.LogError("Player.Instance가 생성되지 않았습니다.");
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _expText.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _expText.SetActive(false);
    }
}
