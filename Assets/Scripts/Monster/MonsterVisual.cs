using UnityEngine;

public class MonsterVisual : MonoBehaviour
{
    [SerializeField] private Transform _firePoint;
    [SerializeField] private GameObject _fireBallPrefab;

    private Animator _animator;
    private SpriteRenderer _spriteRenderer; // Flip x를 체크해주기 위함
    private bool _shoudFireBall = false;



    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // 몬스터 테이블의 몬스터 모델 컬럼에 맞는 컨트롤러를 로드하여 적용
    public void SetVisual(string resourceName)
    {
        var controller = Resources.Load<RuntimeAnimatorController>($"Monsters/{resourceName}");
        if(resourceName != "Demon")
        {
            Flip(true);
        }

        if(controller != null)
        {
            _animator.runtimeAnimatorController = controller;
        }
        else
        {
            Debug.LogWarning("Animator is Null");
        }
    }

    public void PlayAttack() => _animator.SetTrigger("Attack");
    public void PlayFireBall() => _shoudFireBall = true;
    public void PlayHit() => _animator.SetTrigger("Hit");
    public void PlayDie() => _animator.SetTrigger("Die");
    public void PlaySkill() => _animator.SetTrigger("Skill");

    // 애니메이션 이벤트 함수
    public void OnAnimatorEventFireBall()
    {
        if(!_shoudFireBall) return;

        GameObject obj = Instantiate(_fireBallPrefab, _firePoint.position, Quaternion.identity);
        MonsterFireBall script = obj.GetComponent<MonsterFireBall>();
        bool isLeft = GetComponent<SpriteRenderer>().flipX;
        script.Init(isLeft);

        _shoudFireBall = false;
    }

    public void Flip(bool isLeft)
    {
        _spriteRenderer.flipX = isLeft;
    }
}
