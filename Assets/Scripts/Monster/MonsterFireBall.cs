using UnityEngine;

public class MonsterFireBall : MonoBehaviour
{
    [SerializeField] private float _speed = 5f;
    private Animator _animator;
    private bool _isExploded = false;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void Init(bool isLeft)
    {
        if (isLeft)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            _speed = -Mathf.Abs(_speed);
        }
    }

    private void Update()
    {
        if(_isExploded) return;
        transform.Translate(Vector3.right * _speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(_isExploded) return;

        if (collision.CompareTag("Player"))
        {
            Explode();
        }
    }

    private void Explode()
    {
        _isExploded = true;

        if(_animator != null) _animator.SetTrigger("Explode");

        Destroy(gameObject, 1f);
    }
}
