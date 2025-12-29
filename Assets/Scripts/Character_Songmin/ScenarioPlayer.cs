using System.Collections.Generic;
using UnityEngine;

public class ScenarioPlayer : MonoBehaviour
{
    public HashSet<Trigger_Type> _playedScenarios = new();
    bool _isPlaying;
    Dictionary<Trigger_Type, List<ScenarioData>> _scenarioMap;
    private void Start()
    {
        _scenarioMap = new Dictionary<Trigger_Type, List<ScenarioData>>();
        FillScenarioDatas();
    }

    public void FillScenarioDatas()
    {
        _scenarioMap[Trigger_Type.gamestart] = Load("scn_Intro_");
        _scenarioMap[Trigger_Type.dungeonenter] = Load("scn_dun_envy_");
        _scenarioMap[Trigger_Type.stageenter] = Load("scn_dun_envy_st1_");
        _scenarioMap[Trigger_Type.prebattle] = Load("scn_boss_pride_");
        _scenarioMap[Trigger_Type.preseal] = Load("scn_boss_seal_");
        _scenarioMap[Trigger_Type.clear] = Load("scn_pride_clear_");
    }
    List<ScenarioData> Load(string key)
    {
        List<ScenarioData> list = new();
        for (int i = 1; i < int.MaxValue; i++)
        {
            ScenarioData data = DataManager.Instance.GetScenario($"{key}{i:D3}");
            if (data == null) break;
            list.Add(data);
        }
        return list;
    }


    public void RequestScenario(Trigger_Type trigger)
    {
        if (_playedScenarios.Contains(trigger)) return;
        if (_isPlaying) return;
        if (!_scenarioMap.TryGetValue(trigger, out var list)) return;
        if (list.Count == 0) return;

        _isPlaying = true;        

        Player.Instance.SetTalkUI();
        Player.Instance.TalkUI.EnterScenario(list);

        Player.Instance.TalkUI.OnScenarioEnd += OnScenarioEnd;
    }
    void OnScenarioEnd()
    {
        _isPlaying = false;

        _playedScenarios.Add(Player.Instance.CurrentPlayedScenario);

        Player.Instance.TalkUI.OnScenarioEnd -= OnScenarioEnd;

        Player.Instance.OnScenarioFinished();
    }

}