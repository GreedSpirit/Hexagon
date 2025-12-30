using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem; // Input System 필수
using UnityEngine.SceneManagement;

public class DungeonPauseUI : MonoBehaviour
{
    [Header("HandManager")]
    [SerializeField] private HandManager _handManager;

    [Header("Input Settings")]
    [SerializeField] private InputActionReference _escAction; // 인스펙터에서 Battle/Esc 액션 연결

    [Header("UI Components")]
    [SerializeField] private GameObject _menuPanel;
    [SerializeField] private TextMeshProUGUI _dungeonNameText;
    [SerializeField] private Button _resumeButton;
    [SerializeField] private Button _giveUpButton;

    private bool _isPaused = false;

    private void Awake()
    {
        _resumeButton.onClick.AddListener(OnResumeButtonClicked);
        _giveUpButton.onClick.AddListener(OnGiveUpButtonClicked);

        _menuPanel.SetActive(false);
    }

    private void OnEnable()
    {
        if (_escAction != null)
        {
            _escAction.action.Enable();
            _escAction.action.performed += OnEscPerformed;
        }
    }

    private void OnDisable()
    {
        if (_escAction != null)
        {
            _escAction.action.performed -= OnEscPerformed;
            _escAction.action.Disable();
        }
    }

    // ESC 키 입력 시 호출
    private void OnEscPerformed(InputAction.CallbackContext context)
    {
        // 이미 보상 결과창 단계면 팝업 금지
        if (DungeonManager.Instance.IsRewardSequenceActive)
        {
            return;
        }

        if (_isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void PauseGame() // 우측 상단 설정 버튼에도 이 함수 연결 가능
    {
        if (DungeonManager.Instance.IsRewardSequenceActive) return;

        _isPaused = true;

        if (_dungeonNameText != null)
        {
            _dungeonNameText.text = DungeonManager.Instance.GetCurrentDungeonName();
        }

        _menuPanel.SetActive(true);
        _handManager.Deselect();
        Debug.Log("Pause");
        Time.timeScale = 0f;
        //SoundManager.Instance.PlaySFX(SFXType.PopupOpen);
    }

    public void ResumeGame()
    {
        _isPaused = false;
        _menuPanel.SetActive(false);
        Debug.Log("Resume");
        Time.timeScale = 1f;; 
    }

    private void OnResumeButtonClicked()
    {
        ResumeGame();
    }

    // 포기하기 버튼 클릭
    private void OnGiveUpButtonClicked()
    {
        Time.timeScale = 1f;
        _isPaused = false;

        if (Player.Instance != null)
        {
            Player.Instance.BackToVillage();
            Player.Instance.Respawn();
        }
        SceneManager.LoadScene(1);
    }
}