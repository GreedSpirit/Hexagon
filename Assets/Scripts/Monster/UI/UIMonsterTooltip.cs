using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIMonsterTooltip : MonoBehaviour
{
    public static UIMonsterTooltip Instance { get; private set; }

    [Header("UI Components")]
    [SerializeField] private GameObject _tooltipPanel; // 툴팁 전체 패널
    [SerializeField] private TextMeshProUGUI _tooltipText; // 내용을 적을 텍스트
    [SerializeField] private RectTransform _rectTransform; // 패널 위치 제어용(자기 자신)

    [Header("Settings")]
    [SerializeField] private Vector2 _offset = new Vector2(-500, 400); // 몬스터 기준 좌측 상단

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        HideTooltip(); // 시작할 땐 숨김
    }

    public void ShowTooltip(List<MonsterStatusEffectInstance> effects, Vector3 position)
    {
        if (effects == null || effects.Count == 0) return;

        _tooltipPanel.SetActive(true);
        
        // 텍스트 생성
        StringBuilder sb = new StringBuilder();
        foreach (var effect in effects)
        {
            // 효과 이름
            string colorHex = GetColorByBuffType(effect.Type);
            sb.Append($"<color={colorHex}><b>[{DataManager.Instance.GetString(effect.Name).Korean}]</b></color>\n");
            
            // 효과 설명
            string desc = GenerateDescription(effect);
            sb.Append($"{desc}\n\n"); // 줄바꿈으로 구분
        }
        _tooltipText.text = sb.ToString().TrimEnd();

        // 2. 위치 설정
        _rectTransform.position = position;
        _rectTransform.anchoredPosition += _offset; 
    }

    public void HideTooltip()
    {
        _tooltipPanel.SetActive(false);
    }

    // 효과 타입에 따른 제목 색상
    private string GetColorByBuffType(BuffType type)
    {
        switch (type)
        {
            case BuffType.Buff: return "#00FF00";   // 초록 (강화)
            case BuffType.DeBuff: return "#FF0000"; // 빨강 (약화)
            case BuffType.DoT: return "#FFA500";    // 주황 (상태이상/도트)
            default: return "#FFFFFF";
        }
    }

    // 효과 설명을 동적으로 생성하는 로직
    private string GenerateDescription(MonsterStatusEffectInstance effect)
    {
        // 테이블의 Desc를 쓰거나, 로직에 따라 문장 조합
        // 예시: "화상" -> "턴 종료 시 5의 피해를 입습니다."
        string durationText = effect.Duration > 0 ? $"{effect.Duration}Turn" : $"{effect.Stack}Stack";
        
        // 간단한 예시 로직 (실제로는 테이블 Desc 활용)
        return $"{effect.Desc} Effect : {durationText}";
    }
}