using UnityEngine.InputSystem;

public interface IInputState
{
    public void OnEnter();
    public void OnExit();
    public void OnMove(InputAction.CallbackContext ctx);
    public void OnInteract(InputAction.CallbackContext ctx);
}