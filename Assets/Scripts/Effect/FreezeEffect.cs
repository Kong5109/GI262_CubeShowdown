using UnityEngine;

public class FreezeEffect : Effect
{
    #region Fields & Properties
    private const float DefaultFreezeDuration = 2f;
    #endregion

    #region Unity Lifecycle
    protected override void Awake()
    {
        Duration = Duration <= 0f ? DefaultFreezeDuration : Duration;
        base.Awake();
    }
    #endregion

    #region Public Methods
    #endregion

    #region Private Methods
    protected override void OnEffectApplied()
    {
        TargetEnemy?.SetStunned(this, true);
    }

    protected override void OnEffectRemoved()
    {
        TargetEnemy?.SetStunned(this, false);
    }
    #endregion
}

