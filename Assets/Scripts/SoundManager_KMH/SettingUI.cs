using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingUI : MonoBehaviour
{
    [SerializeField] GameObject _setting;

    [Header("볼륨 조절 슬라이더")]
    [SerializeField] Slider _masterSlider;
    [SerializeField] Slider _bgmSlider;
    [SerializeField] Slider _sfxSlider;

    [Header("토글")]
    [SerializeField] Toggle _masterToggle;
    [SerializeField] Toggle _bgmToggle;
    [SerializeField] Toggle _sfxToggle;

    [Header("볼륨 수치 텍스트")]
    [SerializeField] TextMeshProUGUI _masterText;
    [SerializeField] TextMeshProUGUI _bgmText;
    [SerializeField] TextMeshProUGUI _sfxText;

    [Header("이미지")]
    [SerializeField] Image _masterIconImage;
    [SerializeField] Image _bgmIconImage;
    [SerializeField] Image _sfxIconImage;

    [Header("아이콘")]
    [SerializeField] Sprite _soundOnSprite;  // 소리 On
    [SerializeField] Sprite _soundOffSprite; // 소리 Off

    public bool isActive => _setting.activeSelf;   // 활성화 상태
    private GameObject _upgradePanel;
    private bool isActiveUpgrade;

    private void Start()
    {
        // 사운드 매니저 없으면 무시
        if (SoundManager.Instance == null) return;

        // 볼륨 슬라이더 연결
        _masterSlider.onValueChanged.AddListener((value) =>
        {
            SoundManager.Instance.SetMasterVolume(value);       // 볼륨 조절
            UpdateVolumeText(_masterText, value);                // 텍스트 갱신
        });
        _bgmSlider.onValueChanged.AddListener((value) =>
        {
            SoundManager.Instance.SetBGMVolume(value);
            UpdateVolumeText(_bgmText, value);
        });
        _sfxSlider.onValueChanged.AddListener((value) =>
        {
            SoundManager.Instance.SetSFXVolume(value);
            UpdateVolumeText(_sfxText, value);
        });

        // 토글 연결
        _masterToggle.onValueChanged.AddListener((isOn) => {
            SoundManager.Instance.SetMasterOn(isOn);         // On이면 소리 재생
            _masterSlider.interactable = isOn;                // On이면 조작 가능
            UpdateToggleIcon(_masterIconImage, isOn);         // 이미지 교체
        });

        _bgmToggle.onValueChanged.AddListener((isOn) => {
            SoundManager.Instance.SetBGMOn(isOn);
            _bgmSlider.interactable = isOn;
            UpdateToggleIcon(_bgmIconImage, isOn);
        });

        _sfxToggle.onValueChanged.AddListener((isOn) => {
            SoundManager.Instance.SetSFXOn(isOn);
            _sfxSlider.interactable = isOn;
            UpdateToggleIcon(_sfxIconImage, isOn);
        });
        // UI 초기화
        InitUI();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 이 씬만
        if (scene.name == "TestBuildScene_LJH")
        {
            // 일단 활성화된 캔버스
            GameObject villageCanvas = GameObject.Find("VillageCanvas");

            // 캔버스의 자식 중 강화 패널
            Transform upgradePanelTrans = villageCanvas.transform.Find("UpgradePanel");

            if (upgradePanelTrans != null)
                _upgradePanel = upgradePanelTrans.gameObject;
        }
        // 다른 씬 가면 일단 끄기
        else
        {
            _setting.SetActive(false);
        }

    }

    private void Update()
    {
        if (_upgradePanel?.activeSelf == true && isActiveUpgrade == false) isActiveUpgrade = true;

        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            // 대화중이면 무시
            if (Player.Instance?.IsTalking == true) return;

            // 배틀씬 제외
            if (SceneManager.GetActiveScene().name == "DungeonBattleScene") return;

            // 강화패널 켜져있으면 무시
            if (isActiveUpgrade == true)
            {
                isActiveUpgrade = false;
                return;
            }

            SetActivePanel();
        }
    }

    private void InitUI()
    {
        // 슬라이더
        if (_masterSlider != null)
            _masterSlider.value = SoundManager.Instance.MasterVolume;
        if (_bgmSlider != null)
            _bgmSlider.value = SoundManager.Instance.BGMVolume;
        if (_sfxSlider != null)
            _sfxSlider.value = SoundManager.Instance.SFXVolume;

        // 볼륨 텍스트
        UpdateVolumeText(_masterText, _masterSlider.value);
        UpdateVolumeText(_bgmText, _bgmSlider.value);
        UpdateVolumeText(_sfxText, _sfxSlider.value);

        // 상태
        _masterToggle.isOn = SoundManager.Instance.IsMasterOn;
        _bgmToggle.isOn = SoundManager.Instance.IsBGMOn;
        _sfxToggle.isOn = SoundManager.Instance.IsSFXOn;

        // 슬라이더 잠금 상태
        _masterSlider.interactable = _masterToggle.isOn;
        _bgmSlider.interactable = _bgmToggle.isOn;
        _sfxSlider.interactable = _sfxToggle.isOn;

        // 아이콘 이미지
        UpdateToggleIcon(_masterIconImage, _masterToggle.isOn);
        UpdateToggleIcon(_bgmIconImage, _bgmToggle.isOn);
        UpdateToggleIcon(_sfxIconImage, _sfxToggle.isOn);
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
            targetImage.sprite = isOn ? _soundOnSprite : _soundOffSprite;
        }
    }

    // UI 활성화 상태 변경
    public void SetActivePanel()
    {
        // 패널 켜기
        _setting.SetActive(!_setting.activeSelf);
        // 패널 켜면 UI 초기화
        if (_setting.activeSelf) InitUI();
    }

    // 종료 확인
    public void ClickExitApply()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif

        Application.Quit();
    }
}
