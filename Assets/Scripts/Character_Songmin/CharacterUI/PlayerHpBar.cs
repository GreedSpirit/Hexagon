using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class PlayerHpBar : MonoBehaviour
{
    [SerializeField] private Slider _hpBar;
    [SerializeField] private Slider _hitBar;
    [SerializeField] private Slider _poisonBar;
    [SerializeField] private Slider _burnBar;

    float _preHpValue;
    float _hitTarget;
    Coroutine _hitRoutine;

    public void Start()
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


    public void OnDestroy()
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


    public void UpdateHpBar(int currentHp, int Hp, int poison, int burn)
    {        
        
        float hpValue = Mathf.Max(0f, (float)(currentHp - poison - burn) / Hp);
        float burnValue = Mathf.Max(0, (float)(currentHp - poison) / Hp);
        float poisonValue = Mathf.Max(0, (float)currentHp / Hp);
        _hitTarget = poisonValue;

        //if (_preHpValue > hpValue)
        //{            
            _hpBar.value = hpValue;
            _burnBar.value = burnValue;
            _poisonBar.value = poisonValue;
            

            if (_hitRoutine != null)
            {
                StopCoroutine(_hitRoutine);
            }
            _hitRoutine = StartCoroutine(HitBarAnimation());
            _preHpValue = hpValue;
            return;
        //}
        //else
        //{
        //    if (_hitRoutine != null)
        //    {
        //        StopCoroutine(_hitRoutine);
        //    }
        //    _hitRoutine = StartCoroutine(HealBarAnimation(hpValue, burnValue, poisonValue));
        //    _preHpValue = hpValue;
        //    return;
        //}
    }


    public IEnumerator HitBarAnimation()
    {
        while (_hitBar.value != _hitTarget)
        {
            _hitBar.value = Mathf.Lerp(_hitBar.value, _hitTarget, Time.deltaTime * 5f);
            if (Mathf.Abs(_hitBar.value - _hitTarget) < 0.002f)
            {
                _hitBar.value = _hitTarget; break;
            }
            yield return null;
        }
    }

    //public IEnumerator HealBarAnimation(float hpTarget, float burnTarget, float poisonTarget)
    //{
    //    while (_hitBar.value != _hitTarget)
    //    {
    //        _hpBar.value = Mathf.Lerp(_hpBar.value, hpTarget, Time.deltaTime * 5f);
    //        _burnBar.value = Mathf.Lerp(_burnBar.value, burnTarget, Time.deltaTime * 5f);
    //        _poisonBar.value = Mathf.Lerp(_poisonBar.value, poisonTarget, Time.deltaTime * 5f);
    //        _hitBar.value = Mathf.Lerp(_hitBar.value, _hitTarget, Time.deltaTime * 5f);
    //        if (Mathf.Abs(_hitBar.value - _hitTarget) < 0.002f)
    //        {
    //            _hitBar.value = _hitTarget; break;
    //        }
    //        yield return null;
    //    }
    //}

}