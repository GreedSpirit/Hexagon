using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class SoundSettingUI : MonoBehaviour
{
    [Header("볼륨 조절 슬라이더")]
    [SerializeField] Slider masterSlider;
    [SerializeField] Slider bgmSlider;
    [SerializeField] Slider sfxSlider;

    [Header("토글")]
    [SerializeField] Toggle masterToggle;
    [SerializeField] Toggle bgmToggle;
    [SerializeField] Toggle sfxToggle;
    
    [Header("볼륨 수치 텍스트")]
    [SerializeField] TextMeshProUGUI masterText;
    [SerializeField] TextMeshProUGUI bgmText;
    [SerializeField] TextMeshProUGUI sfxText;

    [Header("이미지")]
    [SerializeField] Image masterIconImage;
    [SerializeField] Image bgmIconImage;
    [SerializeField] Image sfxIconImage;

    [Header("아이콘")]
    [SerializeField] Sprite soundOnSprite;  // 소리 On
    [SerializeField] Sprite soundOffSprite; // 소리 Off

    public bool isActive => gameObject.activeSelf;   // 활성화 상태

    private void Start()
    {
        // 사운드 매니저 없으면 무시
        if (SoundManager.Instance == null) return;

        // 볼륨 슬라이더 연결
        masterSlider.onValueChanged.AddListener((value) =>
        {
            SoundManager.Instance.SetMasterVolume(value);       // 볼륨 조절
            UpdateVolumeText(masterText, value);                // 텍스트 갱신
        });
        bgmSlider.onValueChanged.AddListener((value) =>
        {
            SoundManager.Instance.SetBGMVolume(value);
            UpdateVolumeText(bgmText, value);
        });
        sfxSlider.onValueChanged.AddListener((value) =>
        {
            SoundManager.Instance.SetSFXVolume(value);
            UpdateVolumeText(sfxText, value);
        });

        // 토글 연결
        masterToggle.onValueChanged.AddListener((isOn) => {
            SoundManager.Instance.SetMasterOn(isOn);         // On이면 소리 재생
            masterSlider.interactable = isOn;                // On이면 조작 가능
            UpdateToggleIcon(masterIconImage, isOn);         // 이미지 교체
        });

        bgmToggle.onValueChanged.AddListener((isOn) => {
            SoundManager.Instance.SetBGMOn(isOn);
            bgmSlider.interactable = isOn;
            UpdateToggleIcon(bgmIconImage, isOn);
        });

        sfxToggle.onValueChanged.AddListener((isOn) => {
            SoundManager.Instance.SetSFXOn(isOn);
            sfxSlider.interactable = isOn;
            UpdateToggleIcon(sfxIconImage, isOn);
        });
        // UI 초기화
        InitUI();
    }

    private void InitUI()
    {
        // 슬라이더
        if (masterSlider != null)
            masterSlider.value = SoundManager.Instance.MasterVolume;
        if (bgmSlider != null)
            bgmSlider.value = SoundManager.Instance.BGMVolume;
        if (sfxSlider != null)
            sfxSlider.value = SoundManager.Instance.SFXVolume;

        // 볼륨 텍스트
        UpdateVolumeText(masterText, masterSlider.value);
        UpdateVolumeText(bgmText, bgmSlider.value);
        UpdateVolumeText(sfxText, sfxSlider.value);

        // 상태
        masterToggle.isOn = SoundManager.Instance.IsMasterOn;
        bgmToggle.isOn = SoundManager.Instance.IsBGMOn;
        sfxToggle.isOn = SoundManager.Instance.IsSFXOn;

        // 슬라이더 잠금 상태
        masterSlider.interactable = masterToggle.isOn;
        bgmSlider.interactable = bgmToggle.isOn;
        sfxSlider.interactable = sfxToggle.isOn;

        // 아이콘 이미지
        UpdateToggleIcon(masterIconImage, masterToggle.isOn);
        UpdateToggleIcon(bgmIconImage, bgmToggle.isOn);
        UpdateToggleIcon(sfxIconImage, sfxToggle.isOn);
    }

    // 볼륨 텍스트 갱신
    private void UpdateVolumeText(TextMeshProUGUI text, float value)
    {
        // 0.0001 도 0으로 보이게 반올림
        int percent = Mathf.RoundToInt(value * 100);
        text.text = percent.ToString();
    }

    // 아이콘 교체
    private void UpdateToggleIcon(Image targetImage, bool isOn)
    {
        if (targetImage != null)
        {
            // 이미지 스프라이트 변경
            targetImage.sprite = isOn ? soundOnSprite : soundOffSprite;
        }
    }

    // UI 활성화 상태 변경
    public void SetActivePanel()
    {
        // 패널 켜기
        gameObject.SetActive(!gameObject.activeSelf);
        // 패널 켜면 UI 초기화
        if (gameObject.activeSelf) InitUI();
    }
}
