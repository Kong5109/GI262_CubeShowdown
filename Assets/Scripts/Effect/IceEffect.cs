using UnityEngine;

public class IceEffect : Effect
{
    #region Fields & Properties
    [field: SerializeField, Range(0.1f, 1f)] public float SlowMultiplier { get; private set; } = 0.5f;
    #endregion

    #region Unity Lifecycle
    #endregion

    #region Public Methods
    public override void CopySettingsFrom(Effect source)
    {
        if (source is IceEffect iceSource)
        {
            SlowMultiplier = iceSource.SlowMultiplier;
        }

        base.CopySettingsFrom(source);
    }
    #endregion

    #region Private Methods
    protected override void OnEffectApplied()
    {
        TargetEnemy?.ApplySpeedModifier(this, SlowMultiplier);
    }

    protected override void OnEffectRemoved()
    {
        TargetEnemy?.RemoveSpeedModifier(this);
    }
    #endregion
}

