using UnityEngine;
using UnityEngine.UI;

public class SoundSettingUI : MonoBehaviour
{
    [Header("볼륨 조절 슬라이더")]
    [SerializeField] Slider masterSlider;
    [SerializeField] Slider bgmSlider;
    [SerializeField] Slider sfxSlider;

    public bool isActive => gameObject.activeSelf;   // 활성화 상태

    private void Start()
    {
        // 사운드 매니저 없으면 무시
        if (SoundManager.Instance == null) return;

        // 볼륨 슬라이더 연결
        masterSlider.onValueChanged.AddListener(SoundManager.Instance.SetMasterVolume);
        bgmSlider.onValueChanged.AddListener(SoundManager.Instance.SetBGMVolume);
        sfxSlider.onValueChanged.AddListener(SoundManager.Instance.SetSFXVolume);

        // 매니저에서 볼륨 불러오기
        if (masterSlider != null)
            masterSlider.value = SoundManager.Instance.MasterVolume;
        if (bgmSlider != null)
            bgmSlider.value = SoundManager.Instance.BGMVolume;
        if (sfxSlider != null)
            sfxSlider.value = SoundManager.Instance.SFXVolume;
    }

    // UI 활성화 상태 변경
    public void SetActivePanel()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
}
