using UnityEngine;
using UnityEngine.EventSystems;

public class TultipTarget : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] GameObject _tultip;

    public void OnPointerEnter(PointerEventData eventData)
    {
        _tultip.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _tultip.SetActive(false);
    }
}
