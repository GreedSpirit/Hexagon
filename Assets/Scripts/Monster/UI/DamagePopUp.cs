using UnityEngine;
using TMPro;

public enum DamageType
{
    Normal,
    Poison,
    Burn,
}

public class DamagePopUp : MonoBehaviour
{
    [SerializeField] private TextMeshPro _textMesh;
    
    // 75도 각도로 올라가기 위한 벡터
    private Vector3 _moveVector;
    private float _moveSpeed = 2.0f;
    private float _disappearTimer = 1.0f;
    private float _fadeSpeed = 3.0f;
    private Color _textColor;

    private void Awake()
    {
        // 75도 각도 벡터 계산 (오른쪽 기준 0도에서 75도 회전)
        _moveVector = Quaternion.Euler(0, 0, 75) * Vector3.right;
    }

    public void Setup(int damageAmount, DamageType type)
    {
        _textMesh.text = damageAmount.ToString();
        
        switch (type)
        {
            case DamageType.Normal:
                _textColor = Color.white;
                break;
            case DamageType.Poison:
                _textColor = Color.green;
                break;
            case DamageType.Burn:
                _textColor = new Color(1f, 0.5f, 0f); // 주황색
                break;
        }
        _textMesh.fontSize = 5;
        _textMesh.color = _textColor;
        _disappearTimer = 1.0f; // 1초 뒤 사라짐
    }

    private void Update()
    {
        // 위로(75도) 이동
        transform.position += _moveVector * _moveSpeed * Time.deltaTime;

        // 이동 속도 서서히 감소 (일단 넣었는데 어색하면 추후 삭제)
        _moveSpeed -= _moveSpeed * 2f * Time.deltaTime; 

        _disappearTimer -= Time.deltaTime;
        if (_disappearTimer < 0)
        {
            _textColor.a -= _fadeSpeed * Time.deltaTime;
            _textMesh.color = _textColor;

            if (_textColor.a < 0)
            {
                Destroy(gameObject);
            }
        }
    }
}