using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatusEffectIcon : MonoBehaviour
{
    [SerializeField] Image _iconImage; // 테스트용 설정, 추후 이미지 추가되면 Image로 변경 후 이미지 불러오는 로직 추가.
    [SerializeField] TextMeshProUGUI _turnText;
    StatusEffectData _data;

    public void Init(StatusEffectData data)
    {
        _data = data;
        _iconImage.sprite = DataManager.Instance.GetStatusSprite(data.Img);
    }

    public void UpdateIcon(int turn)
    {
        _turnText.text = turn.ToString();
    }


}
