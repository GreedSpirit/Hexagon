using UnityEngine;

public class Village : MonoBehaviour
{
    public string Name { get; private set; }
    public Vector2 SpawnZone { get; private set; }
    public string Img { get; private set; }
    public string Bgm { get; private set; }

    [SerializeField] NpcTalkSlideUI _talkSlide;
    public Npc TalkingNpc { get; private set; }

    private void Start()
    {
        Init("asd");
    }

    public void Init(string key)
    {
        //var villageData = DataManager.Instance.GetVallage(key);
        //Name = villageData.Name;
        //SpawnZone = new Vector2(villageData.CharSpawnAreaX, villageData.CharSpawnAreaY);
        SpawnZone = new Vector2(-7f, 0);
        //Img = villageData.Img;
        //Bgm = villageData.Bgm;
    }

    public void SetNpcToTalk(Npc npc)
    {
        TalkingNpc = npc;
    }

    public void ShowTalkSlide()
    {
        _talkSlide.SetNpc(TalkingNpc);
        _talkSlide.Show();
    }

    public void HideTalkSlide()
    {
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
        player.EnterScenarioMod();
        player.SetTalkingNpc(TalkingNpc);
        player.TalkUI.EnterTalk(TalkingNpc);
    }



    

}