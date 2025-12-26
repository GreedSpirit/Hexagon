using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.VirtualTexturing;

[RequireComponent(typeof(CircleCollider2D))]
public class Npc : MonoBehaviour, ITalkable
{
    NpcData NpcData { get; set; }

    public GameObject NpcCanvas { get; set; }
    public GameObject NameText {  get; set; }
    public GameObject NameHighlight { get; set; }
    public Vector2 UIPos { get; set; } = new Vector2(0, 2);

    int _currentTalkingIndex = -1; //이거는 제이슨으로 저장시켜야 함

    public string Name { get; private set; }
    public string Desc { get; private set; }
    public CharacterType Type { get; private set; } = CharacterType.npc;

    public Vector2 SpawnPos { get; private set; }

    public string Img { get; private set; }
    public string Model { get; private set; }

    public string[] Talks { get; private set; } = new string[4];

    public bool IsTalking { get; private set; }

    public void Init(string key)
    {
        CharacterData characterData = DataManager.Instance.GetCharacter(key);
        NpcData npcData = DataManager.Instance.GetNpc(key);
        Name = DataManager.Instance.GetString(characterData.Name)?.Korean.Trim('"');        
        Desc = DataManager.Instance.GetString(characterData.Desc)?.Korean;
        SpawnPos = new Vector2(npcData.NpcAreaX, npcData.NpcAreaY);
        gameObject.transform.position = SpawnPos;
        //Name = key; //임시 이름 설정
        //for (int i = 0; i < Talks.Length; i++) //테스트용 대사 넣어두기
        //{
        //    Talks[i] = $"{Name}의 대사 {i+1}번 출력";
        //}
        
        


        //NpcTalkData 클래스 및 DataManager 기능 추가하면 아래쪽 주석 해제하기
        NpcTalkData talkData = DataManager.Instance.GetNpcTalk(key);
        Talks[0] = DataManager.Instance.GetString(talkData.NpcTalk1)?.Korean;
        Talks[1] = DataManager.Instance.GetString(talkData.NpcTalk2)?.Korean;
        Talks[2] = DataManager.Instance.GetString(talkData.NpcTalk3)?.Korean;
        Talks[3] = DataManager.Instance.GetString(talkData.NpcTalk4)?.Korean;

        //이미지랑 모델도 나중에 받아오기

        SetNameText();
        SetWord();
        Debug.Log($"{Name} 생성 완료");
    }


    public string GetName()
    {
        return Name;
    }

    public string GetTalk()
    {
        return Talks[_currentTalkingIndex];
    }
    public string GetImage()
    {
        return Img;
    }



    private void SetWord()
    {
        int random = Random.Range(0, Talks.Length);
        if (Talks.Length <= 1)
        {
            _currentTalkingIndex = 0;
            return;
        }
        while (_currentTalkingIndex == random)
        {
            random = Random.Range(0, Talks.Length);
        }
        _currentTalkingIndex = random;
    }

    private void SetNameText()
    {
        NameText.GetComponent<TextMeshProUGUI>().text = Name;
    }

    public void HighlightName(bool readyToShow)
    {
        NameHighlight.SetActive(readyToShow);
    }

    private void OnOffCanvas(bool readyToShow)
    {
        if (readyToShow)
        {
            NpcCanvas.SetActive(true);
        }
        else
        {
            NpcCanvas.SetActive(false);
        }

    }

    public void SwitchIsTalking(bool talking)
    {
        IsTalking = talking;
        OnOffCanvas(!talking);
    }
}
