using UnityEngine;

public class EnemyChasingState : EnemyBaseState
{
    #region Fields & Properties
    private const float MinMoveThresholdSqr = 0.001f;
    private const float crossFadeDuration = 0.1f;
    private const string locomotionAnimation = "Locomotion";

    private static readonly int MoveXHash = Animator.StringToHash("MoveX");
    private static readonly int MoveYHash = Animator.StringToHash("MoveY");

    private Vector2 _currentMoveInput;
    #endregion
    public EnemyChasingState(EnemyStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {

        if (stateMachine?.Animator != null)
        {
            stateMachine.Animator.CrossFadeInFixedTime(locomotionAnimation, crossFadeDuration);
        }
    }

    public override void Exit()
    {
    }

    public override void Tick(float deltaTIme)
    {
        Player target = stateMachine?.Enemy?.Target;
        if (target == null)
        {
            UpdateAnimation(Vector2.zero, deltaTIme);
            return;
        }

        Vector3 direction = target.transform.position - stateMachine.transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < MinMoveThresholdSqr)
        {
            UpdateAnimation(Vector2.zero, deltaTIme);
            return;
        }

        Vector3 moveDir = direction.normalized;
        Move(moveDir, deltaTIme);
        FaceTarget(moveDir, deltaTIme);
        UpdateAnimation(new Vector2(moveDir.x, moveDir.z), deltaTIme);

        if (stateMachine.Enemy.AttackTriggerRange >= (stateMachine.transform.position - target.transform.position).magnitude)
        {
            stateMachine.SwitchState(new EnemyAttackState(stateMachine));
        }
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

        stateMachine.Controller.Move(dir * (stateMachine.Enemy.EffectiveSpeed * deltaTime));
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
}

