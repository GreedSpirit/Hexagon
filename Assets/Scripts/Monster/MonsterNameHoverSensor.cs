using System;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 호버를 감지해야하는 오브젝트에 부착하는 클래스 MVP에서 View 역할
/// Main Camera에 Physics 2D Raycaster 컴포넌트가 있어야 작동함
/// 호버가 시작되면 OnEnter 이벤트 발생, 호버가 끝나면 OnExit 이벤트 발생
/// Collider가 있거나 Image에 Raycast Target이 켜져 있어야 함
/// </summary>

public class MonsterNameHoverSensor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public event Action OnEnter;
    public event Action OnExit;

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnEnter?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnExit?.Invoke();
    }
}
