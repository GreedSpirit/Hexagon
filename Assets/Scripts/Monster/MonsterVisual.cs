using UnityEngine;

public class MonsterVisual : MonoBehaviour
{
    private Animator _animator;
    private SpriteRenderer _spriteRenderer; // Flip x를 체크해주기 위함

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // 몬스터 테이블의 몬스터 모델 컬럼에 맞는 컨트롤러를 로드하여 적용
    public void SetVisual(string resourceName)
    {
        var controller = Resources.Load<RuntimeAnimatorController>($"Monsters/{resourceName}");
        if(resourceName != "ModelMoster01")
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
    public void PlayHit() => _animator.SetTrigger("Hit");
    public void PlayDie() => _animator.SetTrigger("Die");
    public void PlaySkill() => _animator.SetTrigger("Skill");

    public void Flip(bool isLeft)
    {
        _spriteRenderer.flipX = isLeft;
    }
}
