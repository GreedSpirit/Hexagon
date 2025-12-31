using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DungeonPresenter : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private DeckUI _deckUI; // Inspector에서 DeckUI 오브젝트 연결

    [SerializeField] private GameObject _dungeonPanel;

    [SerializeField] private Button _enterButton;
    [SerializeField] private Button _returnButton;

    [SerializeField] private List<DungeonSlotView> _dungeonSlotViews;

    private DungeonData _selectedDungeonData = null;

    void Start()
    {
        _enterButton.onClick.AddListener(OnEnterButtonClicked);
        _returnButton.onClick.AddListener(ShowTown);

        //SetUpDungeonList();
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
        _dungeonPanel.SetActive(false);
        DungeonSessionData.SelectedDungeonId = -1;
        Player.Instance.BackToVillage();
        Player.Instance.Currentvillage.VillageManager.OnOffVillageName(true);
    }

    public void ShowDungeon() // 특정 위치에 다가가거나 버튼을 눌렀을 때 던전 선택 화면으로 전환
    {
        SetUpDungeonList();

        _enterButton.interactable = false;
        _dungeonPanel.SetActive(true);

        OnDungeonSlotClicked(-1); // 초기화: 선택된 던전 없음
        Player.Instance.EnterDungeonSelect();
        Player.Instance.Currentvillage.VillageManager.OnOffVillageName(false);
    }

    private void OnEnterButtonClicked()
    {
        if (_selectedDungeonData != null)
        {
            Debug.Log($"덱 편성 화면으로 이동: {_selectedDungeonData.Name}");

            DungeonSessionData.SelectedDungeonId = _selectedDungeonData.Id;

            // 던전 선택창을 끄고, 덱 UI를 불러옵니다.
            _dungeonPanel.SetActive(false); // 필요하다면 끄기

            // 덱 UI 활성화
            _deckUI.gameObject.SetActive(true);

            // 선택된 던전 정보를 덱 UI에 전달
            _deckUI.ReadyForBattle(_selectedDungeonData);
        }
        else
        {
            Debug.LogWarning("입장할 던전이 선택되지 않았습니다.");
        }
    }





}
