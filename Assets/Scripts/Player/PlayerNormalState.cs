using UnityEngine;

public class PlayerNormalState : PlayerBaseState
{
    #region Fields & Properties
    private const float MinMoveThresholdSqr = 0.001f;
    private const float crossFadeDuration = 0.1f;
    private const string locomotionAnimation = "Locomotion";

    private static readonly int MoveXHash = Animator.StringToHash("MoveX");
    private static readonly int MoveYHash = Animator.StringToHash("MoveY");

    private Vector2 _currentMoveInput;
    #endregion

    #region Public Methods
    public PlayerNormalState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        stateMachine.InputManager.OnAttacking += EnterAttackState;

        stateMachine?.Animator.CrossFadeInFixedTime(locomotionAnimation, crossFadeDuration);
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
        if (stateMachine?.InputManager == null) return;

        Vector2 input = stateMachine.InputManager.MoveInput;
        Vector3 worldMoveDir = CalculateCameraRelativeDirection(input);

        Move(worldMoveDir, deltaTIme);
        FaceTarget(worldMoveDir, deltaTIme);
        UpdateAnimation(input, deltaTIme);
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Converts 2D input (WASD) into a 3D movement direction relative to the main camera's horizontal facing plane.
    /// </summary>
    private Vector3 CalculateCameraRelativeDirection(Vector2 input)
    {
        if (input.sqrMagnitude < MinMoveThresholdSqr)
        {
            return Vector3.zero;
        }

        Transform camTransform = stateMachine.MainCameraTransform;
        if (camTransform == null)
        {
            Vector3 fallbackDir = new Vector3(input.x, 0f, input.y);
            return fallbackDir.sqrMagnitude > 1f ? fallbackDir.normalized : fallbackDir;
        }

        Vector3 camForward = camTransform.forward;
        Vector3 camRight = camTransform.right;

        camForward.y = 0f;
        camRight.y = 0f;

        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDir = (camForward * input.y + camRight * input.x);
        return moveDir.sqrMagnitude > 1f ? moveDir.normalized : moveDir;
    }

    /// <summary>
    /// Smoothly rotates the character model towards the movement direction using Slerp, retaining its facing direction when idle.
    /// </summary>
    private void FaceTarget(Vector3 targetDir, float deltaTime)
    {
        if (targetDir.sqrMagnitude < MinMoveThresholdSqr) return;

        Transform targetTransform = stateMachine.CharacterObj != null
            ? stateMachine.CharacterObj.transform
            : stateMachine.transform;

        Quaternion targetRotation = Quaternion.LookRotation(targetDir, Vector3.up);
        targetTransform.rotation = Quaternion.Slerp(
            targetTransform.rotation,
            targetRotation,
            deltaTime * stateMachine.RotationSpeed
        );
    }

    /// <summary>
    /// Moves the CharacterController along the calculated direction scaled by MovementSpeed and deltaTime.
    /// </summary>
    private void Move(Vector3 dir, float deltaTime)
    {
        if (stateMachine?.Controller == null) return;

        stateMachine.Controller.Move(dir * (stateMachine.MovementSpeed * deltaTime));
    }

    /// <summary>
    /// Smoothly interpolates input magnitude into the Animator Blend Tree parameters (MoveX, MoveY) using AnimationBlendSpeed.
    /// </summary>
    private void UpdateAnimation(Vector2 input, float deltaTime)
    {
        if (stateMachine?.Animator == null) return;

        Vector2 targetBlend = new Vector2(0f, input.magnitude);
        _currentMoveInput = Vector2.Lerp(_currentMoveInput, targetBlend, deltaTime * stateMachine.AnimationBlendSpeed);

        stateMachine.Animator.SetFloat(MoveXHash, _currentMoveInput.x);
        stateMachine.Animator.SetFloat(MoveYHash, _currentMoveInput.y);
    }

    /// <summary>
    /// Handles the attack input event by transitioning the state machine to PlayerTargetingState.
    /// </summary>
    private void EnterAttackState()
    {
        stateMachine?.SwitchState(new PlayerTargetingState(stateMachine));
    }
    #endregion
}
