using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VillageManager : MonoBehaviour
{
    
    [SerializeField] NpcTalkSlideUI _talkSlide;
    [SerializeField] TextMeshProUGUI _villageNameUI;

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
        Player.Instance.Respawn(_currentVillage);
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
        if (player.TalkUI == null)
        {
            Debug.LogError("TalkUI가 Player에 아직 등록되지 않았습니다.");
            return;
        }
        player.TalkWithNpc();
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
            npc.NameText = Instantiate(_nameTextPrefab, npc.NpcCanvas.transform, false);            
            npc.NameText.transform.localPosition = npc.UIPos;
            npc.NameHighlight = Instantiate(_nameHighlightPrefab, npc.NpcCanvas.transform, false);
            npc.NameHighlight.transform.localPosition = npc.UIPos;
            npc.Init(npcData.Npc);
        }    
    }
}
