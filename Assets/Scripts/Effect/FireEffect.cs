using UnityEngine;

public class FireEffect : Effect
{
    #region Fields & Properties
    [field: SerializeField] public int DamagePerTick { get; private set; } = 5;
    [field: SerializeField] public float TickInterval { get; private set; } = 1f;

    private const float ReadyTimer = 0f;
    private const float MinTickInterval = 0.05f;
    private const int MinDamage = 0;

    private float _tickTimer;
    #endregion

    #region Unity Lifecycle
    #endregion

    #region Public Methods
    public override void CopySettingsFrom(Effect source)
    {
        if (source is FireEffect fireSource)
        {
            DamagePerTick = fireSource.DamagePerTick;
            TickInterval = fireSource.TickInterval;
        }

        base.CopySettingsFrom(source);
    }
    #endregion

    #region Private Methods
    protected override void OnEffectApplied()
    {
        _tickTimer = Mathf.Max(MinTickInterval, TickInterval);
    }

    protected override void OnTick(float deltaTime)
    {
        _tickTimer -= deltaTime;
        if (_tickTimer > ReadyTimer)
        {
            return;
        }

        _tickTimer += Mathf.Max(MinTickInterval, TickInterval);
        if (DamagePerTick > MinDamage)
        {
            TargetEnemy.TakeDamage(DamagePerTick);
        }
    }
    #endregion
}
