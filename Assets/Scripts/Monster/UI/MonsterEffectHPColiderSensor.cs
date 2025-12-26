using UnityEngine;
using UnityEngine.EventSystems;

public class MonsterEffectHPColiderSensor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] MonsterStatus _monsterStatus;

    public void OnPointerEnter(PointerEventData eventData)
    {
        UIMonsterTooltip.Instance.ShowTooltip(_monsterStatus.StatusEffects, _monsterStatus.gameObject.transform.position);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UIMonsterTooltip.Instance.HideTooltip();
    }
}
