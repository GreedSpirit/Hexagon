using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class ClickDebugger : MonoBehaviour
{
    void Update()
    {
        // 마우스가 없으면 실행 안 함
        if (Mouse.current == null) return;

        // 마우스 왼쪽 버튼을 눌렀을 때
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            // 1. 마우스 위치에 있는 UI들을 다 조사함
            PointerEventData pointerData = new PointerEventData(EventSystem.current);

            // [새 방식] 마우스 위치 가져오기
            pointerData.position = Mouse.current.position.ReadValue();

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            // 2. 결과를 콘솔에 출력
            if (results.Count > 0)
            {
                Debug.Log($" [클릭 감지됨] 총 {results.Count}개의 UI가 마우스 아래에 있음 ");
                foreach (var result in results)
                {
                    Debug.Log($" - 우선순위: {result.sortingOrder} | 깊이: {result.depth} | 이름: <color=yellow>{result.gameObject.name}</color>");
                }
                Debug.Log("--------------------------------------------------");
            }
            else
            {
                Debug.Log(" 마우스 아래에 UI가 아무것도 없음 (허공 클릭)");
            }
        }
    }
}