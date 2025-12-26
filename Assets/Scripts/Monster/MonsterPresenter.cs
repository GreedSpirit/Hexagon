using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 몬스터 프리젠터 클래스 MVP에서 Presenter 역할 추후 기능이 추가되면 view를 추가하고 Presenter에서 로직을 처리하도록 함
/// </summary>
public class MonsterPresenter : MonoBehaviour
{
    [Header("Model")]
    [SerializeField] private MonsterStatus _monsterStatus;

    [Header("View")]
    [SerializeField] private MonsterNameView _monsterNameView;
    [SerializeField] private List<MonsterNameHoverSensor> hoverSensors;

    private int _hoverCount = 0; //호버 중첩 카운트

    void Start()
    {
        // 1. Model 데이터 받기
        //_monsterStatus는 인스펙터에서 할당

        // 2. View 초기화
        _monsterNameView.Init();

        // 3. NameHoverSensor 이벤트 구독
        foreach(var sensor in hoverSensors)
        {
            sensor.OnEnter += HandleHoverEnter;
            sensor.OnExit += HandleHoverExit;
        }
    }

    private void HandleHoverEnter()
    {
        if (string.IsNullOrEmpty(_monsterStatus.MonsterData.Name))
    {
        Debug.LogError($"ID: {_monsterStatus.MonsterData.Id}, Name: {_monsterStatus.MonsterData.Name} 몬스터의 NameKey가 비어있습니다!");
        return;
    }
        _monsterNameView.SetName(DataManager.Instance.GetString(_monsterStatus.MonsterData.Name).Korean);        
        _hoverCount++;
        bool shouldShow = _hoverCount > 0;
        _monsterNameView.HoverNameView(shouldShow);
    }

    private void HandleHoverExit()
    {
        _hoverCount--;
        if(_hoverCount < 0)
            _hoverCount = 0;

        bool shouldShow = _hoverCount > 0;
        _monsterNameView.HoverNameView(shouldShow);
    }

    void OnDestroy()
    {
        // HoverSensor 이벤트 구독 해제
        foreach(var sensor in hoverSensors)
        {
            if(sensor == null) continue;

            sensor.OnEnter -= HandleHoverEnter;
            sensor.OnExit -= HandleHoverExit;
        }
    }
}
