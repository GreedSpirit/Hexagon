using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIMonsterSelectedSkill : MonoBehaviour, IMonsterSkillObserver
{
    [SerializeField] private MonsterStatus _monsterStatus; //몬스터 상태 참조
    [SerializeField] private SpriteDatabase _spriteDatabase;

    [SerializeField] private Image _selectedSkillIcon; //선택된 스킬 아이콘 이미지
    [SerializeField] private TextMeshProUGUI _skillValueText; //스킬 수치 텍스트

    private const string ATK_ICON = "MonAttackIcon_0";
    private const string HEAL_ICON = "F_UI_SkillsTileMap 22x22_107";
    private const string SHIELD_ICON = "F_UI_SkillsTileMap 22x22_90";
    private const string SPELL_ICON = "F_UI_SkillsTileMap 22x22_47";

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

        string targetSpriteName = "";

        //스킬 아이콘 업데이트
        if (skillData != null)
        {
            switch (skillData.CardType)
            {
                case CardType.Attack:
                    targetSpriteName = ATK_ICON;
                    break;
                case CardType.Healing:
                    targetSpriteName = HEAL_ICON;
                    break;
                case CardType.Shield:
                    targetSpriteName = SHIELD_ICON;
                    break;
                default:
                    targetSpriteName = SPELL_ICON;
                    break;
            }
        }

        Sprite newSprite = _spriteDatabase.GetSprite(targetSpriteName);

        if(newSprite != null)
        {
            _selectedSkillIcon.sprite = newSprite;
            _selectedSkillIcon.color = Color.white;
        }
        else
        {
            _selectedSkillIcon.sprite = null;
            _selectedSkillIcon.color = Color.magenta; // 임시 오류 처리
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
