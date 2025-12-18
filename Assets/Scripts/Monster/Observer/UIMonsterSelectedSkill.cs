using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIMonsterSelectedSkill : MonoBehaviour, IMonsterSkillObserver
{
    [SerializeField] private MonsterStatus _monsterStatus; //몬스터 상태 참조
    [SerializeField] private Image[] _skillIcons; //스킬 아이콘 이미지 배열
    [SerializeField] private Image _selectedSkillIcon; //선택된 스킬 아이콘 이미지
    [SerializeField] private TextMeshProUGUI _skillValueText; //스킬 수치 텍스트

    private void Awake()
    {
        _monsterStatus.AddSkillObserver(this);
    }
    private void OnDestroy()
    {
        _monsterStatus.RemoveSkillObserver(this);
    }

    public void OnMonsterSkillSelected(CardData skillData, int skillValue)
    {
        if(skillValue == -1)
        {
            _selectedSkillIcon.sprite = null;
            _selectedSkillIcon.color = new Color(1, 1, 1, 0); //투명 처리
            _skillValueText.text = "";
            return;
        }

        //스킬 아이콘 업데이트
        if (skillData != null)
        {
            switch (skillData.CardType)
            {
                case CardType.Attack:
                    //_selectedSkillIcon.sprite = _skillIcons[0].sprite;
                    _selectedSkillIcon.color = new Color(1, 1, 1, 1); //불투명 처리
                    // 임시로 스킬 아이콘 색깔을 빨간색으로 설정
                    _selectedSkillIcon.color = Color.red;
                    break;
                case CardType.Healing:
                    //_selectedSkillIcon.sprite = _skillIcons[1].sprite;
                    _selectedSkillIcon.color = new Color(1, 1, 1, 1); //불투명 처리
                    // 임시로 스킬 아이콘 색깔을 파란색으로 설정
                    _selectedSkillIcon.color = Color.blue;
                    break;
                case CardType.Shield:
                    //_selectedSkillIcon.sprite = _skillIcons[2].sprite;
                    _selectedSkillIcon.color = new Color(1, 1, 1, 1); //불투명 처리
                    // 임시로 스킬 아이콘 색깔을 초록색으로 설정
                    _selectedSkillIcon.color = Color.green;
                    break;
                default:
                    _selectedSkillIcon.color = new Color(1, 1, 1, 1); //불투명 처리
                    _selectedSkillIcon.color = Color.lavender;
                    break;
            }
        }

        //스킬 수치 텍스트 업데이트
        if (skillData != null)
        {
            _skillValueText.text = skillValue.ToString("N0");
        }
        else
        {
            _skillValueText.text = "";
        }
    }
}
