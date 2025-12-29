using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TalkUI : MonoBehaviour
{
    //페이드 아웃용 검은 패널
    [SerializeField] GameObject _fadePannel;
    UpgradeManager _upgradeManager;
    public Action OnScenarioEnd;
    CanvasGroup _canvasGroup;
    Coroutine _fadeRoutine;  
    Coroutine _effectRoutine;  
    float fadeDuration = 0.3f;
    ITalkable _currentTalking;
    List<ScenarioData> _currentScenario;
    int _currentIndex;
    
    [SerializeField] GameObject _talkPannel;
    [SerializeField] Image _backgroundPannel;
    [SerializeField] Image _leftImg;
    [SerializeField] Image _rightImg;
    [SerializeField] Image _cutImg;


    [SerializeField] TextMeshProUGUI _characterName;
    [SerializeField] TextMeshProUGUI _characterScript;
    [SerializeField] GameObject _upgradeEnterButton;
    [SerializeField] GameObject _escButton;
    
    
    


    private void Awake()
    {
        _canvasGroup = _fadePannel.GetComponent<CanvasGroup>();
        _currentScenario = new List<ScenarioData>();
        _upgradeManager = FindFirstObjectByType<UpgradeManager>();
    }

    private void Start()
    {
        //Player.Instance.SetTalkUI(this);
        //_testButton.SetActive(false);
    }


    //------------------------------------------------------------------

    #region 마을에서 대화용
    public void EnterTalk(ITalkable talkable) 
    {
        SetTalkable(talkable);

        _currentTalking = talkable; 
        _cutImg.gameObject.SetActive(false);
        _rightImg.gameObject.SetActive(false);
        _leftImg.sprite = DataManager.Instance.GetSprite(SpriteType.Character, _currentTalking.GetImage());
        
        
        
        if (_fadeRoutine != null)
        {
            StopCoroutine(_fadeRoutine);
        }
        _fadeRoutine = StartCoroutine(TalkFadeInAndOut(true));
    }

    public void EnterUpgrade()
    {
        _talkPannel.SetActive(false);        
        _upgradeEnterButton.SetActive(false);
        Player.Instance.Currentvillage.VillageManager.SetUpgradePanel(true);  
    }


    public void UpdateTalk(ITalkable talable)
    {
        SetTalkable(talable);
    }

    public void EndTalk()
    {
        if (_fadeRoutine != null)
            StopCoroutine(_fadeRoutine);

        if (_talkPannel.activeSelf)
        {
            _fadeRoutine = StartCoroutine(TalkFadeInAndOut(false));
        }
        else
        {
            Player.Instance.SwitchIsTalking(false);
            Player.Instance.Currentvillage.VillageManager.SetUpgradePanel(false);
            Destroy(gameObject);
        }
    }

    #endregion

    //------------------------------------------------------------------

    public void EnterScenario(List<ScenarioData> datas)
    {        
        _escButton.SetActive(false);
        _currentScenario = datas;
        _currentIndex = 0;


        if (_fadeRoutine != null)
        {
            StopCoroutine(_fadeRoutine);
        }
        _fadeRoutine = StartCoroutine(ScenarioFadeInAndOut(true));
        Debug.Log(datas.Count);
        UpdateScenario();
    }

    public void UpdateScenario()
    {
        if (_currentIndex >= _currentScenario.Count)
        {               
            EndScenario();
            return;
            //시나리오 종료 처리
        }
        ScenarioData data = _currentScenario[_currentIndex];

        SetCutImage(data);
        SetScenarioBackground(data);
        SetScenarioPortrait(data);

      
        
        _characterScript.text = data.Dialogue;
        _currentIndex++;
        //연출도 추가해주기
    }

    public void EndScenario()
    {
        _currentIndex = 0;
        _currentScenario = null;

        if (_fadeRoutine != null)
        {
            StopCoroutine(_fadeRoutine);
        }
        _fadeRoutine = StartCoroutine(ScenarioFadeInAndOut(false));
        Player.Instance.EnterMoveMod();
        
        OnScenarioEnd?.Invoke();
    }





    //-------------------------------------

    private void SetCutImage(ScenarioData data) //마저 작업해야 함.
    {
        if (!string.IsNullOrEmpty(data.Image_ID))
        {
            _cutImg.gameObject.SetActive(true);
            _cutImg.sprite = DataManager.Instance.GetSprite(
                SpriteType.Character,
                data.Image_ID
            );
        }
        else
        {
            _cutImg.sprite = null;
            _cutImg.gameObject.SetActive(false);
        }
    }




    private void SetScenarioPortrait(ScenarioData data)
    {
        if (string.IsNullOrWhiteSpace(data.Npc))
        {
            _characterName.text = "";
            _characterScript.text = data.Dialogue;
            return;
        }
        if (data.Npc == "에리온") //화자가 에리온이라면
        {
            if (_leftImg.sprite == null) // 에리온의 최초 대사라면
            {
                _leftImg.gameObject.SetActive(true); //왼쪽 초상화 오브젝트 활성화
                _leftImg.sprite = DataManager.Instance.GetSprite(SpriteType.Character, data.Character_Image); //왼쪽 초상화에 에리온 이미지 할당
            }            
            _leftImg.gameObject.GetComponent<CanvasGroup>().alpha = 1f; //왼쪽 초상화 알파값 1로 딤드 해제
            

            if (_rightImg.gameObject.activeSelf) // 오른쪽 이미지가 켜져 있다면
            {
                _rightImg.gameObject.GetComponent<CanvasGroup>().alpha = 0.5f; //알파값 조정으로 딤드 처리
            }
            _characterName.text = data.Npc; //이름 텍스트 설정
        }
        else //화자가 에리온이 아니라면(Npc라면)
        {
            if (_rightImg.sprite == null)  //Npc의 최초 대사라면
            {
                _rightImg.gameObject.SetActive(true); //우측 초상화 오브젝트 활성화
                _rightImg.sprite = DataManager.Instance.GetSprite(SpriteType.Character, data.Character_Image); //우측 초상화에 Npc 할당
            }
            _rightImg.gameObject.GetComponent<CanvasGroup>().alpha = 1f; //딤드 처리 복구

            if (_leftImg.gameObject.activeSelf) //좌측 이미지가 켜져 있다면
            {
                _leftImg.gameObject.GetComponent<CanvasGroup>().alpha = 0.5f; //딤드 처리
            }            
            
            _characterName.text = data.Npc; //이름 텍스트 설정
        }
        
    }

    private void SetScenarioBackground(ScenarioData data)
    {
        _backgroundPannel.GetComponent<CanvasGroup>().alpha = 1f;        

        if (!string.IsNullOrEmpty(data.Background))
        {
            //Debug.Log($"[Scenario BG RAW] '{data.Background}'");
            _backgroundPannel.GetComponent<Image>().sprite = DataManager.Instance.GetSprite(SpriteType.Character , data.Background);
            _backgroundPannel.color = Color.white;
        }
        else
        {            
            //Debug.Log("배경 없음");
            _backgroundPannel.GetComponent<Image>().sprite = null;
            _backgroundPannel.color = Color.black;
        }

    }


    private void SetTalkable(ITalkable talkable)
    {
        _characterName.text =  talkable.GetName();
        _characterScript.text =  talkable.GetTalk();
        //_leftImg.GetComponent<Text>().text = talkable.GetImage();
    }



    IEnumerator ScenarioFadeInAndOut(bool isEnter) //true는 시나리오 진입, false는 시나리오 종료.
    {
        yield return FadeRoutine(true);
        
        _talkPannel.SetActive(isEnter);
        _leftImg.gameObject.SetActive(false);
        _rightImg.gameObject.SetActive(false);
        _backgroundPannel.GetComponent<CanvasGroup>().alpha = 1f;
        Player.Instance.SwitchIsTalking(isEnter);

        if (_currentTalking is Npc)
        {
            _currentTalking.SwitchIsTalking(isEnter);
            _upgradeEnterButton.SetActive(isEnter);
        }


        yield return FadeRoutine(false);
        if (!isEnter)
        {
            Destroy(gameObject);
        }
    }


    IEnumerator TalkFadeInAndOut(bool isEnter) //true는 대화 진입, false는 대화 종료.
    {
        yield return FadeRoutine(true);


        _talkPannel.SetActive(isEnter);
        _escButton.SetActive(isEnter);
        Player.Instance.SwitchIsTalking(isEnter);
        
        

        if (_currentTalking.GetName() == "리오넬") //리오넬과 대화할 땐 강화버튼도 켜고 끄기
        {
            _currentTalking.SwitchIsTalking(isEnter);
            _upgradeEnterButton.SetActive(isEnter);
        }


        yield return FadeRoutine(false);
        if (!isEnter)
        {
            Destroy(gameObject);
        }
        
    }

    

    IEnumerator FadeRoutine(bool isFadeIn) //true일 때 까매지고 false일 때 투명해짐(대화창을 가림)
    {
        float start = _canvasGroup.alpha;
        float end = isFadeIn ? 1f : 0f;        
        float time = 0f;
        
        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            _canvasGroup.alpha = Mathf.Lerp(start, end, time / fadeDuration);
            yield return null;
        }
        _canvasGroup.blocksRaycasts = isFadeIn ? true : false;
        _canvasGroup.alpha = end;
    }

}
