using System;
using System.Collections;
using UnityEngine;

public class TurnTimer
{
    public float TurnDuration = 60f;
    public float WarningTime = 15f;

    
    float _remainingTime;
    bool _isRunning;
    bool _isWarned;

    public event Action OnWarning;
    public event Action OnTimeOver;
    public event Action<int> OnSecondChange;

    public IEnumerator Timer()
    {
        _remainingTime = TurnDuration;
        
        _isRunning = true;
        _isWarned = false;
        while (_isRunning)
        {
            OnSecondChange?.Invoke((int)_remainingTime);
            yield return new WaitForSeconds(1f);
            _remainingTime -= 1f;
            
            if (!_isWarned && _remainingTime <= WarningTime)
            {
                OnWarning?.Invoke();
                _isWarned = true;
            }
            if (_remainingTime <= 0)
            {
                _isRunning = false;
                OnTimeOver?.Invoke();
            }            
        }
    }

    public void Stop()
    {
        _isRunning = false;
    }
}
