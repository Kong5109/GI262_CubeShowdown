using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour , Controls.IPlayerActions
{
    public event Action OnAttacking;
    public Vector2 MoveInput {  get; private set; }
    private Controls InputAction;

    private void Awake()
    {
        InputAction = new Controls();
    }

    private void OnEnable()
    {
        InputAction.Player.SetCallbacks(this);

        InputAction.Enable();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed == false) return;

        OnAttacking?.Invoke();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        MoveInput = context.ReadValue<Vector2>();
    }

    /*void Controls.IPlayerActions.OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed == false) return;

        OnAttack.Invoke();
    }

    void Controls.IPlayerActions.OnMove(InputAction.CallbackContext context)
    {
        MoveInput = context.ReadValue<Vector2>();
    }*/
}
