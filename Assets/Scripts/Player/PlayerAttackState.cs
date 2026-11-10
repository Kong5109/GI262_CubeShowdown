using UnityEngine;

public class PlayerAttackState : PlayerBaseState
{
    #region Fields & Properties
    private const string AttackBoolAnimation = "IsAttacking";
    private const string AttackAnimation = "Standing_Draw_Arrow";
    private const int AnimationLayer = 0;
    private const float AnimationCompleteThreshold = 1.0f;
    private const float EarlyCancelThreshold = 0.8f;
    private const float MinMoveThresholdSqr = 0.001f;
    private const float CrossFadeDuration = 0.1f;

    public Enemy Target { get; private set; }

    private bool _hasAnimationStarted;
    #endregion

    #region Public Methods
    public PlayerAttackState(PlayerStateMachine stateMachine, Enemy target = null) : base(stateMachine)
    {
        Target = target;
    }

    public override void Enter()
    {
        _hasAnimationStarted = false;
        FaceTargetEnemy();
        stateMachine?.Animator.CrossFadeInFixedTime(AttackAnimation, CrossFadeDuration);
    }

    public override void Exit()
    {
        stateMachine?.Animator.SetBool(AttackBoolAnimation, false);
    }

    public override void Tick(float deltaTIme)
    {
        if (stateMachine?.Animator == null) return;

        AnimatorStateInfo stateInfo = stateMachine.Animator.GetCurrentAnimatorStateInfo(AnimationLayer);

        if (!_hasAnimationStarted)
        {
            _hasAnimationStarted = IsAnimationStart(stateInfo);
            return;
        }

        if (ShouldCancelWithMovement(stateInfo) || IsAnimationComplete(stateInfo))
        {
            stateMachine.SwitchState(new PlayerNormalState(stateMachine));
        }
    }
    #endregion

    #region Private Methods
    private void FaceTargetEnemy()
    {
        if (Target == null || stateMachine == null) return;

        Transform targetTransform = stateMachine.CharacterObj != null
            ? stateMachine.CharacterObj.transform
            : stateMachine.transform;

        Vector3 direction = Target.transform.position - targetTransform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude > MinMoveThresholdSqr)
        {
            targetTransform.rotation = Quaternion.LookRotation(direction, Vector3.up);
        }
    }

    private bool IsAnimationStart(AnimatorStateInfo stateInfo)
    {
        return stateInfo.IsName(AttackAnimation);
    }

    private bool ShouldCancelWithMovement(AnimatorStateInfo stateInfo)
    {
        if (stateMachine?.InputManager == null) return false;

        bool hasMoveInput = stateMachine.InputManager.MoveInput.sqrMagnitude > MinMoveThresholdSqr;
        return hasMoveInput && stateInfo.normalizedTime > EarlyCancelThreshold;
    }

    private bool IsAnimationComplete(AnimatorStateInfo stateInfo)
    {
        return stateInfo.normalizedTime >= AnimationCompleteThreshold && !stateMachine.Animator.IsInTransition(AnimationLayer);
    }
    #endregion
}
