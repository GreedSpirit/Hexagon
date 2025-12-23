using UnityEngine.Audio;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [SerializeField] AudioMixer _mixer;      // 믹서
    [SerializeField] AudioSource _bgmSource; // 배경음 소스
    [SerializeField] AudioSource _sfxSource; // 효과음 소스


    // 현재 볼륨
    public float MasterVolume { get; private set; } = 0.5f;
    public float BGMVolume { get; private set; } = 0.5f;
    public float SFXVolume { get; private set; } = 0.5f;



    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        // 볼륨 불러오기
        LoadVolumePrefs();
    }


    // 볼륨 불러오기
    void LoadVolumePrefs()
    {
        // 저장된 볼륨 가져오기, 없으면 유지
        MasterVolume = PlayerPrefs.GetFloat("MasterVolume", MasterVolume);
        BGMVolume = PlayerPrefs.GetFloat("BGMVolume", BGMVolume);
        SFXVolume = PlayerPrefs.GetFloat("SFXVolume", SFXVolume);

        // 불러온 값 설정
        SetMasterVolume(MasterVolume);
        SetBGMVolume(BGMVolume);
        SetSFXVolume(SFXVolume);
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


    // 배경음 교체 재생
    public void PlayBGM()
    {
        //_bgmSource.clip = clip;
        //_bgmSource.Play();
    }


    // 효과음 한 번 재생
    public void PlaySFX()
    {
       // _sfxSource.PlayOneShot(clip);
    }
}
