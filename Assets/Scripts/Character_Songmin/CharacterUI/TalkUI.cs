using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class TalkUI : MonoBehaviour
{
    //페이드 아웃용 검은 패널
    [SerializeField] GameObject _fadePannel;
    CanvasGroup _canvasGroup;
    Coroutine _fadeRoutine;    
    float fadeDuration = 0.3f;
    ITalkable _currentTalking;
    List<ScenarioData> _currentScenario;
    int _currentIndex;
    
    [SerializeField] GameObject _talkPannel;
    [SerializeField] Image _backgroundPannel;
    [SerializeField] Image _leftImg;
    [SerializeField] Image _rightImg;
    [SerializeField] TextMeshProUGUI _characterName;
    [SerializeField] TextMeshProUGUI _characterScript;
    [SerializeField] GameObject _upgradeEnterButton;
    [SerializeField] GameObject _upgradePannel;
    [SerializeField] GameObject _testButton;
    
    


    private void Awake()
    {
        _canvasGroup = _fadePannel.GetComponent<CanvasGroup>();
        _currentScenario = new List<ScenarioData>();
    }

    private void Start()
    {
        Player.Instance.SetTalkUI(this);
        _testButton.SetActive(false);
    }

    //마을에서 대화용
    //------------------------------------------------------
    public void EnterTalk(ITalkable talkable) 
    {
        SetTalkable(talkable);
        _currentTalking = talkable; 
        _rightImg.gameObject.SetActive(false);
        if (_fadeRoutine != null)
        {
            StopCoroutine(_fadeRoutine);
        }        
        _fadeRoutine = StartCoroutine(FadeInAndOut(true));
    }

    public void EnterUpgrade()
    {
        _talkPannel.SetActive(false);
        _testButton.SetActive(true);
        _upgradeEnterButton.SetActive(false);
        _upgradePannel.SetActive(true);
    }


    public void UpdateTalk(ITalkable talable)
    {
        SetTalkable(talable);
    }

    public void EndTalk()
    {        
        if (_fadeRoutine != null)
        {
            StopCoroutine(_fadeRoutine);
        }
        if (_talkPannel.activeSelf == true)
        {
            _fadeRoutine = StartCoroutine(FadeInAndOut(false));
        }
        else
        {
            _upgradePannel.SetActive(false);
            _testButton.SetActive(false);
        }        
    }

    public void EnterScenario(List<ScenarioData> datas)
    {
        if (_fadeRoutine != null)
        {
            StopCoroutine(_fadeRoutine);
        }
        _currentScenario = datas;
        _currentIndex = 0;
        //_fadeRoutine = //여기서 해야할 일 : 까만 화면 만들기
        UpdateScenario();
    }

    public void UpdateScenario()
    {
        ScenarioData data = _currentScenario[_currentIndex];
        //_backgroundPannel.sprite = 스프라이트 설정해주기
        //만약에 까만 화면이면 페이드 아웃 해주기
        if (_leftImg.sprite == null && data.Character_Image == "Img_PC_Erion")
        {
            //_leftImg.sprite = ImageManager.Instance.GetSprite(data.Character_Image);
            if (_rightImg.gameObject.activeSelf)
            {
                _rightImg.gameObject.GetComponent<CanvasGroup>().alpha = 0.5f;
            }
            _leftImg.gameObject.SetActive(true);
            _leftImg.gameObject.GetComponent<CanvasGroup>().alpha = 1f;
            _characterName.text = data.Npc;            
        }
        else if (_rightImg.sprite == null)
        {
            //_rightImg.sprite = ImageManager.Instance.GetSprite(data.Character_Image);
            if (_leftImg.gameObject.activeSelf)
            {
                _leftImg.gameObject.GetComponent<CanvasGroup>().alpha = 0.5f;
            }
            _rightImg.gameObject.SetActive(true);
            _rightImg.gameObject.GetComponent<CanvasGroup>().alpha = 1f;
            _characterName.text = data.Npc;            
        }
        else if (data.Character_Image == null)
        {
            _characterName.text = "";
        }
        _characterScript.text = data.Dialogue;
        _currentIndex++;
        //연출도 추가해주기
    }





    //-------------------------------------
    private void SetScenario()
    {

    }


    private void SetTalkable(ITalkable talkable)
    {
        _characterName.text =  talkable.GetName();
        _characterScript.text =  talkable.GetTalk();
        //_leftImg.GetComponent<Text>().text = talkable.GetImage();
    }

    
    



    IEnumerator FadeInAndOut(bool isEnter)
    {
        yield return FadeRoutine(true);
        _talkPannel.SetActive(isEnter);
        Player.Instance.SwitchIsTalking(isEnter);
        if (_currentTalking is Npc)
        {
            _currentTalking.SwitchIsTalking(isEnter);
            _upgradeEnterButton.SetActive(isEnter);
        }
        yield return FadeRoutine(false);
    }

    IEnumerator FadeRoutine(bool isFadeIn)
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

        _canvasGroup.alpha = end;
    }

}
