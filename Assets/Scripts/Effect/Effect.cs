using UnityEngine;

public abstract class Effect : MonoBehaviour
{
    #region Fields & Properties
    [field: SerializeField] public float Duration { get; protected set; } = 3f;

    public float RemainingDuration { get; private set; }
    protected Enemy TargetEnemy { get; private set; }

    private const float ExpiredTime = 0f;
    #endregion

    #region Unity Lifecycle
    protected virtual void Awake()
    {
        TargetEnemy = GetComponentInParent<Enemy>();
        RefreshDuration();
        OnEffectApplied();
    }

    protected virtual void Update()
    {
        if (TargetEnemy == null)
        {
            DestroySelf();
            return;
        }

        OnTick(Time.deltaTime);

        RemainingDuration -= Time.deltaTime;
        if (RemainingDuration <= ExpiredTime)
        {
            DestroySelf();
        }
    }

    protected virtual void OnDestroy()
    {
        OnEffectRemoved();
    }
    #endregion

    #region Public Methods
    public virtual void RefreshDuration()
    {
        RemainingDuration = Duration;
    }

    public virtual void CopySettingsFrom(Effect source)
    {
        if (source == null)
        {
            return;
        }

        Duration = source.Duration;
        RefreshDuration();
        OnEffectApplied();
    }
    #endregion

    #region Private Methods
    protected virtual void OnEffectApplied()
    {
    }

    protected virtual void OnTick(float deltaTime)
    {
    }

    protected virtual void OnEffectRemoved()
    {
    }

    private void DestroySelf()
    {
        if (TargetEnemy != null && gameObject == TargetEnemy.gameObject)
        {
            Destroy(this);
            return;
        }

        Destroy(gameObject);
    }
    #endregion
}
