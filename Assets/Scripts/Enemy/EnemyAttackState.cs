using UnityEngine;

public class EnemyAttackState : EnemyBaseState
{
    #region Fields & Properties
    private const string AttackAnimation = "Standing_Draw_Arrow";
    private const int AnimationLayer = 0;
    private const float AnimationCompleteThreshold = 1.0f;
    private const float CrossFadeDuration = 0.1f;

    private bool _hasAnimationStarted;
    #endregion

    #region Public Methods
    public EnemyAttackState(EnemyStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        _hasAnimationStarted = false;
        stateMachine?.Animator?.CrossFadeInFixedTime(AttackAnimation, CrossFadeDuration);
    }

    public override void Exit()
    {
    }

    public override void Tick(float deltaTIme)
    {
        if (stateMachine?.Animator == null) return;

        AnimatorStateInfo stateInfo = stateMachine.Animator.GetCurrentAnimatorStateInfo(AnimationLayer);

        if (!_hasAnimationStarted)
        {
            _hasAnimationStarted = stateInfo.IsName(AttackAnimation);
            return;
        }

        if (IsAnimationComplete(stateInfo))
        {
            stateMachine.SwitchState(new EnemyChasingState(stateMachine));
        }
    }
    #endregion

    #region Private Methods
    private bool IsAnimationComplete(AnimatorStateInfo stateInfo)
    {
        return stateInfo.normalizedTime >= AnimationCompleteThreshold
            && !stateMachine.Animator.IsInTransition(AnimationLayer);
    }
    #endregion
}
