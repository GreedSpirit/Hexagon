using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class Npc : MonoBehaviour
{
    public NpcTalkSlideUI NpcTalkSlideUI;


    public string Name { get; private set; }
    public string Desc { get; private set; }
    public CharacterType Type { get; private set; } = CharacterType.npc;

    public string Img { get; private set; }
    public string Model { get; private set; }

    public string[] Talks { get; private set; } = new string[4];

    private void Start()
    {
        Init("KeyNpcLibra");
        //Debug.Log($"{Desc}");
    }

    public void Init(string key)
    {
        //CharacterData characterData = DataManager.Instance.GetCharacter(key);
        //Name = DataManager.Instance.GetString(characterData.Name).Korean;
        //Desc = DataManager.Instance.GetString(characterData.Desc).Korean;

        for (int i = 0; i < Talks.Length; i++) //테스트용 대사 넣어두기
        {
            Talks[i] = $"대사 {i+1}번 출력";
        }
        
        


        //NpcTalkData 클래스 및 DataManager 기능 추가하면 아래쪽 주석 해제하기
        //NpcTalkData talkData = DataManager.Instance.GetNpcTalk(characterData.Name);
        //Talks[0] = DataManager.Instance.GetString(talkData.NpcTalk1).Korean;
        //Talks[1] = DataManager.Instance.GetString(talkData.NpcTalk2).Korean;
        //Talks[2] = DataManager.Instance.GetString(talkData.NpcTalk3).Korean;
        //Talks[3] = DataManager.Instance.GetString(talkData.NpcTalk4).Korean;

        //이미지랑 모델도 나중에 받아오기
    }


    public virtual void Interact()
    {
        int random = Random.Range(0, Talks.Length);
        Debug.Log($"{Talks[random]}");
    }
}
