using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine;

public enum BGMType     // 배경음 타입
{
    None,       // 없음
    Title,      // 타이틀
    Village,    // 마을
    Battle,     // 전투
    Boss,       // 보스전
    Reward,     // 보상
}

public enum SFXType     // 효과음 타입 (공용만 - 버튼, 팝업)
{
    Click,      // 클릭
    PopupOpen,  // 팝업 켬
    PopupClose, // 팝업 끔
}


[System.Serializable]
public class BGMData
{
    public string name;         // 구분용 이름
    public BGMType type;        // 타입
    public AudioClip clip;      // 클립
}

[System.Serializable]
public class SFXData
{
    public string name;         // 구분용 이름
    public SFXType type;        // 타입
    public AudioClip clip;      // 클립
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("오디오 믹서")]
    [SerializeField] AudioMixer _mixer;      // 믹서
    [Header("오디오 소스")]
    [SerializeField] AudioSource _bgmSource; // 배경음 소스
    [SerializeField] AudioSource _sfxSource; // 효과음 소스

    [Header("BGM 설정")]
    [SerializeField] float _fadeTime = 1.0f;
    [SerializeField] float _volume = 0.5f;
    [SerializeField] List<BGMData> _bgmList;

    [Header("공용 SFX 설정")]
    [SerializeField] List<SFXData> _sfxList;

    // 현재 볼륨
    public float MasterVolume { get; private set; } = 0.3f;
    public float BGMVolume { get; private set; } = 0.3f;
    public float SFXVolume { get; private set; } = 0.3f;

    // 클립 간단하게 가져올 수 있도록 딕셔너리 생성
    private Dictionary<BGMType, AudioClip> _bgmTable = new Dictionary<BGMType, AudioClip>();
    private Dictionary<SFXType, AudioClip> _sfxTable = new Dictionary<SFXType, AudioClip>();

    // 배경음 코루틴
    private Coroutine _bgmCoroutine;

    // 현재 씬 이름
    private string _currentSceneName;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // 저장된 볼륨 가져오기, 없으면 유지
            MasterVolume = PlayerPrefs.GetFloat("MasterVolume", MasterVolume);
            BGMVolume = PlayerPrefs.GetFloat("BGMVolume", BGMVolume);
            SFXVolume = PlayerPrefs.GetFloat("SFXVolume", SFXVolume);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // 리스트를 딕셔너리로
        foreach (var bgmData in _bgmList)
        {
            if (!_bgmTable.ContainsKey(bgmData.type))
                _bgmTable.Add(bgmData.type, bgmData.clip);
        }

        foreach (var sfxData in _sfxList)
        {
            if (!_sfxTable.ContainsKey(sfxData.type))
                _sfxTable.Add(sfxData.type, sfxData.clip);
        }

        // 불러온 값 설정
        SetMasterVolume(MasterVolume);
        SetBGMVolume(BGMVolume);
        SetSFXVolume(SFXVolume);

        // 씬 로드 델리게이트 구독
        SceneManager.sceneLoaded += OnSceneLoaded;

        // 맨첨에 일단 실행
        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    private void OnDestroy()
    {
        // 파괴될일 없을 것 같지만 그래도 구독 해지
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }


    // 전체
    public void SetMasterVolume(float value)
    {
        // 볼륨 할당
        MasterVolume = value;
        // 믹서 그룹 볼륨 설정
        _mixer.SetFloat("MasterVolume", Mathf.Log10(value) * 20);
        // 저장
        PlayerPrefs.SetFloat("MasterVolume", value);
    }

    // 배경음
    public void SetBGMVolume(float value)
    {
        BGMVolume = value;
        _mixer.SetFloat("BGMVolume", Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat("BGMVolume", value);
    }

    // 효과음
    public void SetSFXVolume(float value)
    {
        SFXVolume = value;
        _mixer.SetFloat("SFXVolume", Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat("SFXVolume", value);
    }

    // 씬 로드시 실행
    // 씬 이름에 맞는 타입의 배경음 재생
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 씬 이름 저장
        _currentSceneName = scene.name;

        // 씬 BGM 재생
        PlaySceneBGM();
    }

    // 현재 씬의 기본 BGM으로 되돌리기
    // 보스전 끝났거나, 특수 상황 끝났을 떄
    public void PlaySceneBGM()
    {
        // 씬이름에 따라 BGM 타입 가져오기
        BGMType defaultType = GetSceneBGMType(_currentSceneName);

        // 재생
        PlayBGM(defaultType);
    }

    // 씬에 따라 BGM 타입 반환
    private BGMType GetSceneBGMType(string sceneName)
    {
        BGMType targetBGM;

        switch (sceneName)
        {
            case "TitleScene":
                targetBGM = BGMType.Title;
                break;
            case "TestBuildScene_LJH":
            case "VillageScene":
                targetBGM = BGMType.Village;
                break;
            case "DungeonBattleScene":
                targetBGM = BGMType.Battle;
                break;
            default:
                targetBGM = BGMType.None;
                break;
        }

        return targetBGM;
    }
    // 배경음 재생 (BGMType)
    public void PlayBGM(BGMType type)
    {
        // None으로 끄기 시도하거나 타입 찾지 못했으면 멈춤
        if (type == BGMType.None)
        {
            // 재생중일 때
            if (_bgmSource.isPlaying)
            {
                //이미 꺼져있으면 무시
                if (_bgmCoroutine != null) StopCoroutine(_bgmCoroutine);
                //아니면 페이드 아웃만 실행
                _bgmCoroutine = StartCoroutine(BGMOff());
            }
            return;
        }

        // 해당 타입의 클립 배열 가져오기
        if (_bgmTable.TryGetValue(type, out AudioClip clip))
        {
            // 다음 클립
            AudioClip nextClip = clip;

            // 이미 재생 중인 배경음이면 무시
            if (_bgmSource.isPlaying && _bgmSource.clip == nextClip)
                return;

            // 코루틴이 돌고 있으면 정지
            if (_bgmCoroutine != null)
                StopCoroutine(_bgmCoroutine);

            // 배경음 교체 코루틴 시작
            _bgmCoroutine = StartCoroutine(ChangeBGM(nextClip));
        }
    }

    // 배경음 변경
    IEnumerator ChangeBGM(AudioClip nextClip)
    {
        // 이미 재생 중일 때만 줄임
        if (_bgmSource.isPlaying)
        {
            while (_bgmSource.volume > 0)
            {
                // 볼륨 내리기       프레임 단위로
                _bgmSource.volume -= Time.deltaTime / _fadeTime;
                yield return null;
            }
        }

        // 다 줄이고 확실히 볼륨 0, 멈추기
        _bgmSource.volume = 0;
        _bgmSource.Stop();

        // Clip 교체, 재생
        _bgmSource.clip = nextClip;
        _bgmSource.Play();

        // 페이드인
        while (_bgmSource.volume < _volume)
        {
            _bgmSource.volume += Time.deltaTime / _fadeTime;
            yield return null;
        }

        // 끝나면 확실히 1로
        _bgmSource.volume = _volume;
    }

    // 배경음 끄기 (페이드 아웃)
    IEnumerator BGMOff()
    {
        while (_bgmSource.volume > 0)
        {
            _bgmSource.volume -= Time.deltaTime / _fadeTime;
            yield return null;
        }
        _bgmSource.volume = 0;
        _bgmSource.Stop();
        _bgmSource.clip = null;
    }





    // 공용 효과음 재생 (SFXType)
    public void PlaySFX(SFXType type)
    {
        if (_sfxTable.TryGetValue(type, out AudioClip clip))
        {
            PlaySFX(clip); // 아래 함수 재활용
        }
    }

    // 일반 재생 (플레이어, 몬스터가 본인 파일 들고 와서 재생)
    public void PlaySFX(AudioClip clip)
    {
        if(clip == null)
        {
            Debug.LogError("AudioClip이 할당되어있지 않습니다.");
            return;
        }
        _sfxSource.PlayOneShot(clip);
    }
}
