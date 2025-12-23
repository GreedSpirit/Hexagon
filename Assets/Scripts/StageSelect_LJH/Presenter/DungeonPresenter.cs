using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DungeonPresenter : MonoBehaviour
{
    [SerializeField] private GameObject _townPanel;
    [SerializeField] private GameObject _dungeonPanel;

    [SerializeField] private Button _enterButton;
    [SerializeField] private Button _returnButton;

    [SerializeField] private List<DungeonSlotView> _dungeonSlotViews;

    private DungeonData _selectedDungeonData = null;

    void Start()
    {
        _enterButton.onClick.AddListener(OnEnterButtonClicked);
        _returnButton.onClick.AddListener(ShowTown);

        SetUpDungeonList();
    }

    void SetUpDungeonList()
    {
        foreach(var slotview in _dungeonSlotViews)
        {
            slotview.Initialize(DataManager.Instance.GetDungeon(slotview.DungeonId), OnDungeonSlotClicked);
        }
    }

    void OnDungeonSlotClicked(int dungeonId)
    {
        if(dungeonId == -1)
        {
            _selectedDungeonData = null;
            // 모든 던전 슬롯 뷰 선택 해제
            foreach(var slotview in _dungeonSlotViews)
            {
                slotview.SetSelected(false);
            }
            return;
        }
        _selectedDungeonData = DataManager.Instance.GetDungeon(dungeonId);
        Debug.Log($"선택된 던전: {_selectedDungeonData.Name}");
        
        // 선택된 던전 슬롯 뷰 업데이트
        foreach(var slotview in _dungeonSlotViews)
        {
            //! 캐릭터 레벨에 따른 입장 가능 여부 처리 필요
            slotview.SetSelected(slotview.DungeonId == dungeonId);
        }

        if(_selectedDungeonData != null)
        {
            _enterButton.interactable = true;
        }
        else
        {
            _enterButton.interactable = false;
        }
    }

    private void ShowTown() // 마을 화면으로 전환
    {
        _townPanel.SetActive(true);
        _dungeonPanel.SetActive(false);
        DungeonSessionData.SelectedDungeonId = -1;
    }

    public void ShowDungeon() // 특정 위치에 다가가거나 버튼을 눌렀을 때 던전 선택 화면으로 전환
    {
        _enterButton.interactable = false;
        _townPanel.SetActive(false);
        _dungeonPanel.SetActive(true);

        OnDungeonSlotClicked(-1); // 초기화: 선택된 던전 없음
    }

    private void OnEnterButtonClicked()
    {
        if(_selectedDungeonData != null)
        {
            Debug.Log($"던전 입장: {_selectedDungeonData.Name}");
            // 던전 입장 로직 추가
            DungeonSessionData.SelectedDungeonId = _selectedDungeonData.Id;
            SceneManager.LoadScene("DungeonBattleScene"); // 예시: 던전 씬 로드 추후 이름을 이용한 씬 이름으로 이동
            //SceneManager.LoadScene(_selectedDungeonData.Name);
        }
        else
        {
            Debug.LogWarning("입장할 던전이 선택되지 않았습니다.");
            return;
        }
    }





}
