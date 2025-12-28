using UnityEngine;

public class DamageTextManager : MonoBehaviour
{
    public static DamageTextManager Instance { get; private set; }

    [SerializeField] private DamagePopUp _damagePopupPrefab;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // DamageTextManager.Instance.ShowDamage(damage, transform, type) 넣으면 바로 뜰 수 있도록
    public void ShowDamage(int damage, Vector3 position, DamageType type = DamageType.Normal)
    {
        Vector3 spawnPosition = position + new Vector3(0.5f, 0.5f, 0); 
        
        // 아주 약간의 랜덤 위치 오차 부여 (조금 더 역동적이게 보이지 않을까? 추후 삭제될수도)
        spawnPosition.x += Random.Range(-0.1f, 0.1f);
        spawnPosition.y += Random.Range(-0.1f, 0.1f);

        DamagePopUp popup = Instantiate(_damagePopupPrefab, spawnPosition, Quaternion.identity);
        popup.Setup(damage, type);
    }
}