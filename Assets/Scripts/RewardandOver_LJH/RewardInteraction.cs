using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;
using System;

public class RewardInteraction : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject _interactionPanel;
    [SerializeField] private Image _gaugeFillImage;

    [Header("Input Action")]
    [SerializeField] private InputActionReference _mashAction;

    [Header("Settings")]
    [SerializeField] private float _increaseAmount = 0.05f;
    [SerializeField] private float _decreaseSpeed = 0.2f;
    [SerializeField] private float _timeoutDuration = 5f; // 5초 타임아웃
    [SerializeField] private AudioClip _seal;
    [SerializeField] private AudioClip _sealComplete;

    private float _currentValue = 0f;
    private Coroutine _decayCoroutine;
    private Coroutine _timeoutCoroutine;
    
    public Action OnInteractionComplete; // 게이지 완료 혹은 타임아웃 시 호출

    private void OnEnable()
    {
        _mashAction.action.performed += OnSpacePressed;
    }

    private void OnDisable()
    {
        _mashAction.action.performed -= OnSpacePressed;
        StopAllCoroutines();
    }

    public void StartInteraction()
    {
        _currentValue = 0f;
        _gaugeFillImage.fillAmount = 0f;
        _interactionPanel.SetActive(true);
        
        _mashAction.action.Enable();
        
        // 코루틴 시작
        StartDecay();
        StartTimeoutTimer();
    }

    private void OnSpacePressed(InputAction.CallbackContext context)
    {
        _currentValue = Mathf.Min(_currentValue + _increaseAmount, 1f);
        SoundManager.Instance.PlaySFX(_seal);
        UpdateUI();

        // 입력이 들어올 때마다 타임아웃 타이머 리셋
        StartTimeoutTimer();

        if (_currentValue >= 1f)
        {
            FinishInteraction();
        }
    }

    private IEnumerator DecayRoutine()
    {
        while (true)
        {
            if (_currentValue > 0)
            {
                _currentValue -= _decreaseSpeed * Time.deltaTime;
                _currentValue = Mathf.Max(_currentValue, 0);
                UpdateUI();
            }
            yield return null;
        }
    }

    private IEnumerator TimeoutRoutine()
    {
        // 5초 동안 대기
        yield return new WaitForSeconds(_timeoutDuration);
        
        // 5초 동안 입력이 없으면 자동으로 완료 처리
        Debug.Log("5초간 입력 없음: 자동 진행");
        FinishInteraction();
    }

    private void StartDecay()
    {
        if (_decayCoroutine != null) StopCoroutine(_decayCoroutine);
        _decayCoroutine = StartCoroutine(DecayRoutine());
    }

    private void StartTimeoutTimer()
    {
        if (_timeoutCoroutine != null) StopCoroutine(_timeoutCoroutine);
        _timeoutCoroutine = StartCoroutine(TimeoutRoutine());
    }

    private void UpdateUI() => _gaugeFillImage.fillAmount = _currentValue;

    private void FinishInteraction()
    {
        StopAllCoroutines();
        _mashAction.action.Disable();
        _interactionPanel.SetActive(false);
        Player.Instance.EnterReward();
        SoundManager.Instance.PlaySFX(_sealComplete);
        OnInteractionComplete?.Invoke();
    }
}