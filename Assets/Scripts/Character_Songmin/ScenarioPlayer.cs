using System.Collections.Generic;
using UnityEngine;

public class ScenarioPlayer : MonoBehaviour
{
    List<ScenarioData> _gamestart = new List<ScenarioData>();
    List<ScenarioData> _dungeonenter = new List<ScenarioData>();
    List<ScenarioData> _stageenter = new List<ScenarioData>();
    List<ScenarioData> _prebattle = new List<ScenarioData>();
    List<ScenarioData> _preseal = new List<ScenarioData>();
    List<ScenarioData> _clear = new List<ScenarioData>();

    public void FillScenarioDatas()
    {
        for (int i = 1; i < int.MaxValue; i++)
        {
            ScenarioData data = DataManager.Instance.GetScenario($"scn_Intro_" + i.ToString("D3"));
            if (data == null)
            {
                break;
            }
            _gamestart.Add(data);
        }
        for (int i = 1; i < int.MaxValue; i++)
        {
            ScenarioData data = DataManager.Instance.GetScenario($"scn_dun_envy_" + i.ToString("D3"));
            if (data == null)
            {
                break;
            }
            _dungeonenter.Add(data);
        }
        for (int i = 1; i < int.MaxValue; i++)
        {
            ScenarioData data = DataManager.Instance.GetScenario($"scn_dun_envy_st1_" + i.ToString("D3"));
            if (data == null)
            {
                break;
            }
            _stageenter.Add(data);
        }
        for (int i = 1; i < int.MaxValue; i++)
        {
            ScenarioData data = DataManager.Instance.GetScenario($"scn_boss_pride_" + i.ToString("D3"));
            if (data == null)
            {
                break;
            }
            _prebattle.Add(data);
        }
        for (int i = 1; i < int.MaxValue; i++)
        {
            ScenarioData data = DataManager.Instance.GetScenario($"scn_boss_seal_" + i.ToString("D3"));
            if (data == null)
            {
                break;
            }
            _preseal.Add(data);
        }
        for (int i = 1; i < int.MaxValue; i++)
        {
            ScenarioData data = DataManager.Instance.GetScenario($"scn_pride_clear_" + i.ToString("D3"));
            if (data == null)
            {
                break;
            }
            _clear.Add(data);
        }
    }

    public void EnterScenario(Trigger_Type Trigger_Type)
    {
        switch (Trigger_Type)
        {
            case Trigger_Type.gamestart:
                Player.Instance.TalkUI.EnterScenario(_gamestart);
                break;
            case Trigger_Type.dungeonenter:
                Player.Instance.TalkUI.EnterScenario(_dungeonenter);
                break;
            case Trigger_Type.stageenter:
                Player.Instance.TalkUI.EnterScenario(_stageenter);
                break;
            case Trigger_Type.prebattle:
                Player.Instance.TalkUI.EnterScenario(_prebattle);
                break;
            case Trigger_Type.preseal:
                Player.Instance.TalkUI.EnterScenario(_preseal);
                break;
            case Trigger_Type.clear:
                Player.Instance.TalkUI.EnterScenario(_clear);
                break;
            default:
                Debug.Log("None 형식의 시나리오는 재생할 수 없습니다.");
                break;
        }
    }
}