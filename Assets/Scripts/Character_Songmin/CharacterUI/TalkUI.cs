using System.Collections;
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

    //
    [SerializeField] GameObject _talkPannel;
    [SerializeField] Text _img;
    [SerializeField] TextMeshProUGUI _characterName;
    [SerializeField] TextMeshProUGUI _characterScript;
    [SerializeField] GameObject _upgradeEnterButton;
    [SerializeField] GameObject _upgradePannel;
    [SerializeField] GameObject _testButton;
    

    private void Awake()
    {
        _canvasGroup = _fadePannel.GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        Player.Instance.SetTalkUI(this);
        _testButton.SetActive(false);
    }

    public void EnterTalk(ITalkable talable)
    {
        SetTalkable(talable);
        _currentTalking = talable;
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









    //-------------------------------------
    
    private void SetTalkable(ITalkable talkable)
    {
        _characterName.text =  talkable.GetName();
        _characterScript.text =  talkable.GetTalk();
        _img.text = talkable.GetImage();
    }


    IEnumerator FadeInAndOut(bool isEnter)
    {
        yield return FadeRoutine(true);
        _talkPannel.SetActive(isEnter);
        if (_currentTalking is Npc)
        {
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
