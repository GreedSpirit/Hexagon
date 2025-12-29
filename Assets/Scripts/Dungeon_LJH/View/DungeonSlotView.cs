using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DungeonSlotView : MonoBehaviour
{
    [Header("ID Settings")]
    [SerializeField] public int DungeonId;

    [Header("UI")]
    [SerializeField] private Button _dungeonButton;
    [SerializeField] private TextMeshProUGUI _dungeonNameText;
    [SerializeField] private int _requiredLevel;
    [SerializeField] private GameObject _yellowBorderObject;
    // 던전 선택 시 Presenter에게 알리기 위한 이벤트
    public UnityAction<int> OnClickAction;

    public void Initialize(DungeonData data, UnityAction<int> onClick)
    {
        _dungeonNameText.text = DataManager.Instance.GetString(data.Name).Korean.Trim();
        _requiredLevel = data.RequiredLevel;
        OnClickAction = onClick;

        _dungeonButton.onClick.AddListener(() => OnClickAction?.Invoke(DungeonId));

        SetSelected(false);

        //임의로 플레이어 레벨 3으로 설정(현재 플레이어가 던전 선택창에 없기 때문)
        //int playerLevel = Player.Instance.GetLevel();
        int playerLevel = 3;
        if(playerLevel < _requiredLevel)
        {
            _dungeonButton.interactable = false;
        }
    }

    public void SetSelected(bool isSelected)
    {
        _yellowBorderObject.SetActive(isSelected); //노란 테두리 만들기
        transform.localScale = isSelected ? Vector3.one * 1.2f : Vector3.one; //확대 연출
    }
}

