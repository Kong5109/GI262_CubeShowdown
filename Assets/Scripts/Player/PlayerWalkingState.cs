using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWalkingState : PlayerBaseState
{
    #region Fields & Properties
    private static readonly int MoveXHash = Animator.StringToHash("MoveX");
    private static readonly int MoveYHash = Animator.StringToHash("MoveY");

    private Vector2 _currentMoveInput;
    #endregion

    #region Public Methods
    public PlayerWalkingState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        stateMachine.InputManager.OnAttacking += EnterAttackState;

        if (stateMachine?.Animator != null)
        {
            _currentMoveInput = new Vector2(
                stateMachine.Animator.GetFloat(MoveXHash),
                stateMachine.Animator.GetFloat(MoveYHash)
            );
        }
    }

    public override void Exit()
    {
        stateMachine.InputManager.OnAttacking -= EnterAttackState;
    }

    public override void Tick(float deltaTIme)
    {
        if (stateMachine?.InputManager == null || stateMachine.Animator == null) return;

        Vector2 targetInput = stateMachine.InputManager.MoveInput;
        float blendSpeed = stateMachine.AnimationBlendSpeed;

        MoveAnimation(_currentMoveInput, targetInput, deltaTIme, blendSpeed);
    }
    #endregion

    #region Private Methods

    private void MoveAnimation(Vector2 currentMoveInput, Vector2 targetInput,float deltaTime,float blendSpeed )
    {
        _currentMoveInput = Vector2.Lerp(currentMoveInput, targetInput, deltaTime * blendSpeed);

        stateMachine.Animator.SetFloat(MoveXHash, _currentMoveInput.x);
        stateMachine.Animator.SetFloat(MoveYHash, _currentMoveInput.y);
    }

    private void EnterAttackState()
    {
        stateMachine?.SwitchState(new PlayerAttackState(stateMachine));
    }

    #endregion
}
