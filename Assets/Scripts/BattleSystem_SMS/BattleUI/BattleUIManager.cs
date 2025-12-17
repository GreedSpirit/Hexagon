using TMPro;
using UnityEngine;

public class BattleUIManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _timerText;
    [SerializeField] GameObject _timerObject;

    [SerializeField] TextMeshProUGUI _turnText;
    [SerializeField] GameObject _turnUI;

    public TurnTimer Timer { get; private set; }
    public int TurnCount { get; private set; } = 0;

    private void Awake()
    {
        Timer = new TurnTimer();
    }
    private void Start()
    {
        Timer.OnWarning += TimeTextWarning;
        Timer.OnTimeOver += StopTimer;
        Timer.OnSecondChange += SetTimerText;
    }
    private void OnDestroy()
    {
        Timer.OnWarning -= TimeTextWarning;
        Timer.OnTimeOver -= StopTimer;
        Timer.OnSecondChange -= SetTimerText;
    }


    public void StartTimer() //플레이어 턴 시작 시 배틀매니저에 연결해서 실행
    {
        Timer.Stop();
        StartCoroutine(Timer.Timer());
    }

    public void StopTimer() //플레이어 턴 끝날 시 배틀 매니저에서 실행
    {
        Timer.Stop();
        _timerObject.SetActive(false);
    }


    public void CountTurn() //플레이어 턴 시작 시 배틀매니저에 연결해서 실행.
    {
        TurnCount++;
        if (_turnUI.activeSelf == false)
        {
            TurnUIOn();
        }
        _turnText.text = $"{TurnCount} 턴";
    }

    public void OffTurnUI()
    {
        _turnUI.SetActive(false);
    }








    private void TurnUIOn()
    {
        _turnUI.SetActive(true);
    }

    private void TimeTextWarning()
    {
        _timerObject.SetActive(true);
    }
    
    private void SetTimerText(int time)
    {
        _timerText.text = time.ToString();
    }

    
}
