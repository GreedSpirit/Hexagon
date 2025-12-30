using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VillageManager : MonoBehaviour
{
    
    [SerializeField] NpcTalkSlideUI _talkSlide;
    [SerializeField] TextMeshProUGUI _villageNameUI;
    [SerializeField] GameObject _villageNameUIObject;
    [SerializeField] GameObject _upgradePanel;

    [SerializeField] GameObject _testButtons;

    [SerializeField] RuntimeAnimatorController _libra;
    [SerializeField] RuntimeAnimatorController _lionel;

    Npc _currentTalkNpc;

    List<VillageData> _villageDatas = new List<VillageData>();
    Dictionary<string, Village> _villages = new Dictionary<string, Village>();

    //List<NpcData> _npcDatas = new List<NpcData>();
    //Dictionary<string, Npc> _npcs = new Dictionary<string, Npc>();

    Village _currentVillage;

    [SerializeField] GameObject _npcPrefab;
    [SerializeField] GameObject _npcCanvasPrefab;
    [SerializeField] GameObject _nameTextPrefab;
    [SerializeField] GameObject _nameHighlightPrefab;


    private void Start()
    {
        MakeAllVillageDatas();
        MakeAllVillages();
        ChangeVillage("실렌시아");

        Player.Instance.Respawn();
        StartCoroutine(PlayIntroAfterReady());
    }




    public void ChangeVillage(string vilName)
    {
        _currentVillage = _villages[vilName];
        Player.Instance.SetVillage(_currentVillage);
        _villageNameUI.text = _currentVillage.Name;
        MakeAllNpc(_currentVillage.VillageData);
    }


    public void ShowTalkSlide(Npc npc)
    {        
        _currentTalkNpc = npc;
        _talkSlide.SetNpc(npc);
        _currentTalkNpc.HighlightName(true);
        _talkSlide.Show();
    }

    public void HideTalkSlide()
    {
        _currentTalkNpc.HighlightName(false);
        _talkSlide.Hide();
    }

    public void TalkInteractClick()
    {
        Player player = Player.Instance;
        player.TalkWithNpc();        
        HideTalkSlide();
        
    }

    public void OnOffTalkSlide(bool Onoff)
    {
        if (_talkSlide != null)
        _talkSlide?.gameObject?.SetActive(Onoff);
    }
    public void OnOffVillageName(bool Onoff)
    {
        if (_villageNameUIObject != null)
        _villageNameUIObject.SetActive(Onoff);
    }


    private void MakeAllVillageDatas()
    {
        for (int i = 1; i < int.MaxValue; i++)
        {
            VillageData data = DataManager.Instance.GetVillage(i);
            if (data == null)
            {
                return;
            }
            _villageDatas.Add(data);
        }        
    }
    private void MakeAllVillages()
    {
        foreach (var data in _villageDatas)
        {
            Village newVillage = MakeVillage(data);
            _villages.Add(newVillage.Name, newVillage);
        }
    }

    private Village MakeVillage(VillageData data)
    {
        Village newVillage = new Village(data, this);                
        return newVillage;
    }

    private void MakeAllNpc(VillageData villageData)
    {
        for (int i = 1; i < int.MaxValue; i++)
        {
            NpcData npcData = DataManager.Instance.GetNpc(i);
            if (npcData == null)
            {
                break;
            }
            if (npcData.Village != villageData.Key)
            {
                continue;
            }
            GameObject npcObject = Instantiate(_npcPrefab);            
            Npc npc = npcObject.GetComponent<Npc>();                        
            npc.NpcCanvas = Instantiate(_npcCanvasPrefab, npcObject.transform, false);
            npc.NpcCanvas.transform.localPosition = npc.UIPos;
            npc.NameText = Instantiate(_nameTextPrefab, npc.NpcCanvas.transform, false);            
            npc.NameHighlight = Instantiate(_nameHighlightPrefab, npc.NpcCanvas.transform, false);
            npc.Init(npcData.Npc);
            switch (npc.Name)
            {
                case "리브라":
                    npc.SetAnimator(_libra);
                    break;
                case "리오넬":
                    npc.SetAnimator(_lionel);
                    break;
            }
        }    
    }
    private IEnumerator PlayIntroAfterReady()
    {
        // ScenarioPlayer 초기화 대기
        while (!Player.Instance.GetComponent<ScenarioPlayer>().IsInitialized)
            yield return null;

        if (!Player.Instance.GetComponent<ScenarioPlayer>()
        .IsScenarioPlayed(Trigger_Type.gamestart))
        {
            Player.Instance.PlayScenarioGuaranteed(
                Trigger_Type.gamestart,
                () => Player.Instance.EnterMoveMod()
            );
        }
        else
        {
            Player.Instance.EnterMoveMod();
        }
    }
    public void SetUpgradePanel(bool on)
    {
        _upgradePanel.SetActive(on);
        _testButtons.SetActive(on);
    }
}
