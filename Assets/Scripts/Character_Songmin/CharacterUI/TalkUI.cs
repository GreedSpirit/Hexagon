using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TalkUI : MonoBehaviour
{
    //페이드 아웃용 검은 패널
    [SerializeField] GameObject _fadePannel;
    CanvasGroup _canvasGroup;
    float fadeDuration = 0.3f;
    Coroutine _fadeRoutine;

    //
    [SerializeField] GameObject _talkPannel;
    [SerializeField] Text _img;
    [SerializeField] TextMeshProUGUI _characterName;
    [SerializeField] TextMeshProUGUI _characterScript;
    [SerializeField] GameObject _reinforceButton;
    

    private void Awake()
    {
        _canvasGroup = _fadePannel.GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        Player.Instance.SetTalkUI(this);        
    }

    public void EnterTalk(ITalkable talable)
    {
        SetTalkable(talable);
        if (_fadeRoutine != null)
            StopCoroutine(_fadeRoutine);
        _fadeRoutine = StartCoroutine(FadeInAndOut(true));
    }

    public void UpdateTalk(ITalkable talable)
    {
        SetTalkable(talable);
    }

    public void EndTalk()
    {        
        if (_fadeRoutine != null)
            StopCoroutine(_fadeRoutine);
        _fadeRoutine = StartCoroutine(FadeInAndOut(false));
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
