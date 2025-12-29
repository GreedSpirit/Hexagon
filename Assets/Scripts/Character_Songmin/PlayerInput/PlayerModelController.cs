using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Rigidbody2D))]
[RequireComponent (typeof(BoxCollider2D))]
public class PlayerModelController : MonoBehaviour
{
    Rigidbody2D _rigid;
    Vector2 _moveInput;
    List<Npc> _neerNpcs = new List<Npc>();    
    Animator _animator;
    Vector2 _originScale;


    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _rigid = GetComponent<Rigidbody2D>();
        _rigid.gravityScale = 0;
        _rigid.freezeRotation = true;
        _originScale = transform.localScale;
    }

    private void FixedUpdate()
    {        
        Move();                
    }


    public void SetMoveInput(Vector2 direction)
    {
        _moveInput = direction;
    }


    public void Move()
    {
        if (_moveInput != Vector2.zero)
        {
            
            _animator.SetBool("IsWalk", true);
            if (_moveInput.x > 0)
            {
                gameObject.transform.localScale = new Vector2(_originScale.x, _originScale.y);
            }
            else if (_moveInput.x < 0)
            {
                gameObject.transform.localScale = new Vector2(-_originScale.x, _originScale.y);
            }            
            Vector2 movePos = _rigid.position + _moveInput * Player.Instance.GetMoveSpeed() * Time.fixedDeltaTime;
            _rigid.MovePosition(movePos);
            //gameObject.transform.Translate(_moveInput * Time.deltaTime * Player.Instance.GetMoveSpeed());
        }
        else
        {
            _animator.SetBool("IsWalk", false);
        }
    }

    public void ResetModelScale()
    {
        gameObject.transform.localScale = _originScale;
    }

    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Npc npc))
        {
            if (!_neerNpcs.Contains(npc))
            {
                _neerNpcs.Add(npc);
            }            
            Player.Instance.CanInteract = true;
            CheckNpcDistance();
            Player.Instance.Currentvillage.VillageManager.ShowTalkSlide(_neerNpcs[0]);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Npc npc))
        {
            _neerNpcs.Remove(npc);

            Player.Instance.CanInteract = _neerNpcs.Count > 0;

            if (_neerNpcs.Count > 0)
            {
                CheckNpcDistance();
                Player.Instance.Currentvillage.VillageManager.ShowTalkSlide(_neerNpcs[0]);
            }
            else
            {
                Player.Instance.SetTalkingNpc(null);
                Player.Instance.Currentvillage.VillageManager.HideTalkSlide();
            }
        }
    }

    private void CheckNpcDistance()
    {
        float lastDistance = float.MaxValue;
        foreach (var npc in _neerNpcs)
        {
            if (npc == null)
            {
                continue;
            }

            float distance = Vector2.Distance(transform.position, npc.transform.position);
            if (distance < lastDistance)
            {
                lastDistance = distance;                
                Player.Instance.SetTalkingNpc(npc);
                Debug.Log(npc.Name);
            }
        }
    }
}