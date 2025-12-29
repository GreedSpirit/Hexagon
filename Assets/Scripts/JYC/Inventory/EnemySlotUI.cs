using UnityEngine;
using UnityEngine.UI;
using System;

public class EnemySlotUI : MonoBehaviour
{
    [SerializeField] private Image _monsterImage;
    [SerializeField] private GameObject _selectionOutline;
    [SerializeField] private Button _button;

    private MonsterData _data;
    private Action<MonsterData> _onClickCallback;

    public void Init(MonsterData data, Action<MonsterData> onClick)
    {
        _data = data;
        _onClickCallback = onClick;

        // InventoryDB에서 이미지 가져오기
        if (_monsterImage != null)
        {
            Sprite spr = DataManager.Instance.GetInventorySprite(data.Img);

            if (spr != null)
            {
                _monsterImage.sprite = spr;
                _monsterImage.color = Color.white;
            }
            else
            {
                // 이미지가 없으면 분홍색으로 표시 (오류 확인용)
                _monsterImage.color = Color.magenta;
            }
        }

        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(OnClick);

        SetSelected(false);
    }

    private void OnClick()
    {
        _onClickCallback?.Invoke(_data);
        SetSelected(true); // 클릭 시 바로 선택 표시 켜기
    }

    public void SetSelected(bool isSelected)
    {
        if (_selectionOutline != null)
            _selectionOutline.SetActive(isSelected);
    }
}
