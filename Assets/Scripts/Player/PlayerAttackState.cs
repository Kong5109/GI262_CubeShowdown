using UnityEngine;

public class PlayerAttackState : PlayerBaseState
{
    #region Fields & Properties
    private const string AttackBoolAnimation = "IsAttacking";
    private const string AttackAnimation = "Standing_Draw_Arrow";
    private const int AnimationLayer = 0;
    private const float AnimationCompleteThreshold = 0.95f;

    private bool _hasAnimationStarted;
    #endregion

    #region Public Methods
    public PlayerAttackState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        _hasAnimationStarted = false;
        stateMachine?.Animator.SetBool(AttackBoolAnimation, true);
    }

    public override void Exit()
    {
        stateMachine?.Animator.SetBool(AttackBoolAnimation, false);
    }

    public override void Tick(float deltaTIme)
    {
        if (stateMachine?.Animator == null) return;

        AnimatorStateInfo stateInfo = stateMachine.Animator.GetCurrentAnimatorStateInfo(AnimationLayer);

        if (!_hasAnimationStarted && stateInfo.IsName(AttackAnimation))
        {
            _hasAnimationStarted = true;
        }

        if (_hasAnimationStarted && stateInfo.normalizedTime >= AnimationCompleteThreshold && !stateMachine.Animator.IsInTransition(AnimationLayer))
        {
            stateMachine.SwitchState(new PlayerWalkingState(stateMachine));
        }
    }
    #endregion
}
