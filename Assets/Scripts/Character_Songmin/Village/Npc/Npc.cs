using TMPro;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class Npc : MonoBehaviour, ITalkable
{
    [SerializeField] TextMeshProUGUI _nameText;
    [SerializeField] GameObject _nameHighlight;

    int _currentTalkingIndex = -1; //이거는 제이슨으로 저장시켜야 함

    public string Name { get; private set; }
    public string Desc { get; private set; }
    public CharacterType Type { get; private set; } = CharacterType.npc;

    public string Img { get; private set; }
    public string Model { get; private set; }

    public string[] Talks { get; private set; } = new string[4];

    

    private void Start()
    {
        Init("KeyNpcLibra");    
        SetWord();
        //Debug.Log($"{Desc}");
    }

    public void Init(string key)
    {
        //CharacterData characterData = DataManager.Instance.GetCharacter(key);
        //Name = DataManager.Instance.GetString(characterData.Name).Korean;
        //Desc = DataManager.Instance.GetString(characterData.Desc).Korean;

        Name = key; //임시 이름 설정
        for (int i = 0; i < Talks.Length; i++) //테스트용 대사 넣어두기
        {
            Talks[i] = $"{Name}의 대사 {i+1}번 출력";
        }
        
        


        //NpcTalkData 클래스 및 DataManager 기능 추가하면 아래쪽 주석 해제하기
        //NpcTalkData talkData = DataManager.Instance.GetNpcTalk(characterData.Name);
        //Talks[0] = DataManager.Instance.GetString(talkData.NpcTalk1).Korean;
        //Talks[1] = DataManager.Instance.GetString(talkData.NpcTalk2).Korean;
        //Talks[2] = DataManager.Instance.GetString(talkData.NpcTalk3).Korean;
        //Talks[3] = DataManager.Instance.GetString(talkData.NpcTalk4).Korean;

        //이미지랑 모델도 나중에 받아오기

        SetNameText();
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
        _nameText.text = Name;
    }

    public void HighlightName(bool readyToShow)
    {
        _nameHighlight.SetActive(readyToShow);
    }
}
