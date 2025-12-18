using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHpBar : MonoBehaviour
{
    [SerializeField] private Slider _hpBar;
    [SerializeField] private Slider _hitBar;
    [SerializeField] private Slider _poisonBar;
    [SerializeField] private Slider _burnBar;

    float _hitTarget;
    Coroutine _hitRoutine;

    public void OnEnable()
    {                
        if (Player.Instance != null)
        {
            Player.Instance.OnHpChanged += UpdateHpBar;
            Player.Instance.PushHp();
        }
        else
        {
            Debug.LogError("Player.Instance가 생성되지 않았습니다.");
        }
        
    }

    public void UpdateHpBar(int currentHp, int Hp, int poison, int burn)
    {
        if (currentHp > 0)
        {
            float hpValue = Mathf.Max(0, (float)(currentHp - poison - burn) / Hp);
            float burnValue = Mathf.Max(0, (float)(currentHp - poison) / Hp);
            float poisonValue = Mathf.Max(0, (float)currentHp / Hp);


            _hpBar.value = hpValue;
            _burnBar.value = burnValue;
            _poisonBar.value = poisonValue;

            _hitTarget = hpValue;
        }
        else
        {
            _hpBar.value = 0;
        }
        if (_hitRoutine != null)
        {
            StopCoroutine(_hitRoutine);
        }
        _hitRoutine = StartCoroutine(BarAnimation());
    }

    public IEnumerator BarAnimation()
    {
        while (_hitBar.value > _hitTarget)
        {
            _hitBar.value = Mathf.Lerp(_hitBar.value, _hitTarget, Time.deltaTime * 5f);
            
            if (Mathf.Abs(_hitBar.value - _hitTarget) < 0.002f)
            {
                _hitBar.value = _hitTarget;
                break;
            }

            yield return null;
        }
    }



    public void OnDisable()
    {
        if (Player.Instance != null)
        {
            Player.Instance.OnHpChanged -= UpdateHpBar;
        }
        else
        {
            Debug.LogError("Player.Instance가 생성되지 않았습니다.");
        }
    }
}
