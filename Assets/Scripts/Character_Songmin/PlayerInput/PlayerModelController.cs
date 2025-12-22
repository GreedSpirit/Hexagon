using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerModelController : MonoBehaviour
{
    
    Vector2 _moveInput;
    
    private void Update()
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
            if (_moveInput.x > 0)
            {
                gameObject.transform.localScale = new Vector2(1, 1);
            }
            else if (_moveInput.x < 0)
            {
                gameObject.transform.localScale = new Vector2(- 1, 1);
            }
            gameObject.transform.Translate(_moveInput * Time.deltaTime * Player.Instance.GetMoveSpeed());
        }            
    }

    public void Interact()
    {
        Debug.Log("상호작용!");
    }
}