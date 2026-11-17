using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour, Controls.IPlayerActions
{
    #region Fields & Properties
    [field: SerializeField] public Vector2 MoveInput { get; private set; }

    public event Action OnAttacking;

    private Controls _inputAction;
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        _inputAction = new Controls();
    }

    private void OnEnable()
    {
        _inputAction.Player.SetCallbacks(this);
        _inputAction.Enable();
    }

    private void OnDisable()
    {
        _inputAction.Disable();
    }

    private void OnDestroy()
    {
        _inputAction.Dispose();
    }
    #endregion

    #region Public Methods
    public void OnAttack(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        OnAttacking?.Invoke();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        MoveInput = context.ReadValue<Vector2>();
    }

    public void OnAttack()
    {
        OnAttacking?.Invoke();
    }
    #endregion

    #region Private Methods
    #endregion
}
