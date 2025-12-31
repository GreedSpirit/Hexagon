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
    CanvasGroup _backgroundCanvasGroup;
    Coroutine _fadeRoutine;
    Coroutine _cutImageRoutine;
    Coroutine _shakeRoutine;

    Vector2 _shakeOrigin;
    float _shakeDuration = 0.3f;

    float _fadeDuration = 0.3f;
    ITalkable _currentTalking;
    List<ScenarioData> _currentScenario;
    int _currentIndex;
    string _currentCutImageKey;

    [SerializeField] GameObject _talkPannel;
    [SerializeField] Image _backgroundPannel;
    [SerializeField] Image _leftImg;
    [SerializeField] Image _rightImg;
    [SerializeField] Image _cutImg;


    [SerializeField] TextMeshProUGUI _characterName;
    [SerializeField] TextMeshProUGUI _characterScript;
    [SerializeField] GameObject _characterNameBox;
    [SerializeField] GameObject _upgradeEnterButton;
    [SerializeField] GameObject _escButton;
    [SerializeField] GameObject _spaceBarText;

    bool _isScenarioTransitioning;



    private void Awake()
    {
        _backgroundCanvasGroup = _fadePannel.GetComponent<CanvasGroup>();
        _currentScenario = new List<ScenarioData>();
        _upgradeManager = FindFirstObjectByType<UpgradeManager>();
        _shakeOrigin = _backgroundPannel.transform.localPosition;
    }

    private void Start()
    {
        //Player.Instance.SetTalkUI(this);
        //_testButton.SetActive(false);
    }
    private void OnDestroy()
    {
        if (Player.Instance != null && Player.Instance.TalkUI == this)
            Player.Instance.TalkUI = null;
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

    public void EscButtonClicked()
    {
        Player.Instance.EndTalk();
    }

    #endregion

    //------------------------------------------------------------------

    public void EnterScenario(List<ScenarioData> datas)
    {        
        _currentCutImageKey = null;
        _cutImg.sprite = null;
        _cutImg.gameObject.SetActive(false);
        _rightImg.gameObject.SetActive(false);


        _currentTalking = null;
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
        if (_isScenarioTransitioning)
            return;

        if (_currentScenario == null || _currentScenario.Count == 0)
            return;

        if (_currentIndex >= _currentScenario.Count)
        {
            EndScenario();
            return;
        }

        StartCoroutine(UpdateScenarioRoutine());
    }
    IEnumerator UpdateScenarioRoutine()
    {
        _isScenarioTransitioning = true;

        ScenarioData data = _currentScenario[_currentIndex];

        SetCutImage(data);
        SetScenarioBackground(data);
        SetScenarioPortrait(data);

        if (data.Event_Type == Event_Type.shake)
        {
            if (_shakeRoutine != null)
            {
                StopCoroutine(_shakeRoutine);
            }
                

            _shakeRoutine = StartCoroutine(ShakeRoutine());
        }

        _characterScript.text = data.Dialogue;
        _currentIndex++;

        // 최소 연출 대기 시간
        yield return new WaitForSeconds(0.3f);

        _isScenarioTransitioning = false;
    }


    public void EndScenario()
    {
        _currentIndex = 0;
        _currentScenario = null;

        if (_fadeRoutine != null)
            StopCoroutine(_fadeRoutine);

        _fadeRoutine = StartCoroutine(ScenarioFadeInAndOut(false));

        
        // OnScenarioEnd?.Invoke();
    }





    //-------------------------------------

    private void SetCutImage(ScenarioData data)
    {
        if (_cutImageRoutine != null)
        {
            StopCoroutine(_cutImageRoutine);
        }
        _cutImageRoutine = StartCoroutine(CutImageRoutine(data));
    }




    private void SetScenarioPortrait(ScenarioData data)
    {
        if (string.IsNullOrWhiteSpace(data.Npc)) // 화자 없음 = 나레이션
        {
            _characterName.text = "";
            _characterNameBox.SetActive(false);

         
            if (_rightImg.gameObject.activeSelf)
            {
                _rightImg.gameObject.SetActive(false);
            }

            _characterScript.text = data.Dialogue;
            return;
        }

        Sprite newSprite = DataManager.Instance.GetSprite(SpriteType.Character, data.Character_Image);
        

        if (data.Npc == "에리온") // 화자가 에리온일 때
        {
            if (!_characterNameBox.activeSelf)
            {
                _characterNameBox.SetActive(true);
            }

            _leftImg.gameObject.SetActive(true);

            
            if (_leftImg.sprite != newSprite) // 스프라이트가 다르면 교체
            {
                _leftImg.sprite = newSprite;
            }

            _leftImg.GetComponent<CanvasGroup>().alpha = 1f;

            if (_rightImg.gameObject.activeSelf)
            {
                _rightImg.GetComponent<CanvasGroup>().alpha = 0.5f;
            }

            _characterName.text = data.Npc;
        }
        else // 화자가 NPC일 때
        {
            if (newSprite == null)
            {
                _rightImg.gameObject.SetActive(false);
                return;
            }
            if (!_characterNameBox.activeSelf)
            {
                _characterNameBox.SetActive(true);
            }

            _rightImg.gameObject.SetActive(true);

            // sprite가 다를 때만 교체
            if (_rightImg.sprite != newSprite)
            {
                _rightImg.sprite = newSprite;
            }

            _rightImg.GetComponent<CanvasGroup>().alpha = 1f;

            if (_leftImg.gameObject.activeSelf)
            {
                _leftImg.GetComponent<CanvasGroup>().alpha = 0.5f;
            }

            _characterName.text = data.Npc;
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



    IEnumerator ScenarioFadeInAndOut(bool isEnter)
    {
        yield return FadeRoutine(true);

        _talkPannel.SetActive(isEnter);
        _leftImg.gameObject.SetActive(false);
        _rightImg.gameObject.SetActive(false);
        _backgroundPannel.GetComponent<CanvasGroup>().alpha = 1f;

        
        Player.Instance.EnterScenarioMod();

        yield return FadeRoutine(false);

        if (!isEnter)
        {
            OnScenarioEnd?.Invoke();
            OnScenarioEnd = null;
            Destroy(gameObject);
        }
    }





    IEnumerator TalkFadeInAndOut(bool isEnter) //true는 대화 진입, false는 대화 종료.
    {
        yield return FadeRoutine(true);


        _talkPannel.SetActive(isEnter);
        _escButton.SetActive(isEnter);
        _spaceBarText.SetActive(!isEnter);
        _characterNameBox.SetActive(isEnter);
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
        float start = _backgroundCanvasGroup.alpha;
        float end = isFadeIn ? 1f : 0f;        
        float time = 0f;
        
        while (time < _fadeDuration)
        {
            time += Time.deltaTime;
            _backgroundCanvasGroup.alpha = Mathf.Lerp(start, end, time / _fadeDuration);
            yield return null;
        }
        _backgroundCanvasGroup.blocksRaycasts = isFadeIn ? true : false;
        _backgroundCanvasGroup.alpha = end;
    }
    //--------------------------------------------------------------------------------------------



    //-------------------------------------------------------------------------------------------
    //컷 이미지용 연출 코루틴
    IEnumerator CutImageRoutine(ScenarioData data)
    {
        _isScenarioTransitioning = true;
        if (string.IsNullOrEmpty(_currentCutImageKey) && string.IsNullOrEmpty(data.Image_ID))
        {
            yield break;
        }

        if (string.IsNullOrEmpty(_currentCutImageKey) && !string.IsNullOrEmpty(data.Image_ID))
        {
            yield return FadeInCutImage(data.Image_ID);
            yield break;
        }

        if (!string.IsNullOrEmpty(_currentCutImageKey) && string.IsNullOrEmpty(data.Image_ID))
        {
            yield return FadeOutCutImage();
            yield break;
        }

        if (!string.IsNullOrEmpty(_currentCutImageKey) && !string.IsNullOrEmpty(data.Image_ID))
        {
            if (_currentCutImageKey == data.Image_ID)
            {
                yield break;
            }
            yield return FadeOutCutImage();
            yield return FadeInCutImage(data.Image_ID);
            yield break;
        }
        _isScenarioTransitioning = false;
    }

    IEnumerator FadeInCutImage(string key)
    {
        _cutImg.gameObject.SetActive(true);
        _cutImg.sprite = DataManager.Instance.GetSprite(SpriteType.Character, key);

        CanvasGroup canvasGroup = _cutImg.gameObject.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;

        float t = 0f;
        while (t < _fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, t / _fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 1;
        _currentCutImageKey = key;
    }

    IEnumerator FadeOutCutImage()
    {
        CanvasGroup canvasGroup = _cutImg.gameObject.GetComponent<CanvasGroup>();
        float start = canvasGroup.alpha;

        float t = 0f;
        while (t < _fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(start, 0f, t / _fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 0;
        _cutImg.sprite = null;
        _cutImg.gameObject.SetActive(false);

        _currentCutImageKey = null;
    }

    //------------------------------------------------------------------------------
    // 배경 진동용 연출 코루틴

    IEnumerator ShakeRoutine()
    {
        float time = 0f;

        RectTransform rect = _backgroundPannel.rectTransform;
        float _shakeStrength = 10f;

        while (time < _shakeDuration)
        {
            time += Time.deltaTime;

            float offsetX = UnityEngine.Random.Range(-_shakeStrength, _shakeStrength);
            float offsetY = UnityEngine.Random.Range(-_shakeStrength, _shakeStrength);

            rect.localPosition = _shakeOrigin + new Vector2(offsetX, offsetY);

            yield return null;
        }

        // 원위치 복구 (중요)
        rect.localPosition = _shakeOrigin;
    }

}
