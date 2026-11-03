using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWalkingState : PlayerBaseState
{
    public PlayerWalkingState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
    }

    public override void Exit()
    {
    }

    public override void Tick(float deltaTIme)
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            stateMachine?.SwitchState(new PlayerAttackState(stateMachine));
        }

        Vector3 moveInput;
        moveInput.x = Input.GetAxis("Horizontal");
        moveInput.y = Input.GetAxis("Vertical");

        stateMachine?.Animator.SetFloat("MoveX", moveInput.x);
        stateMachine?.Animator.SetFloat("MOveY", moveInput.y);
    }
}
