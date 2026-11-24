using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class PlayerEnterTriggerEvent : MonoBehaviour
{
    #region Fields & Properties
    [field: SerializeField] public bool TriggerOnce { get; private set; }
    [field: SerializeField] public UnityEvent OnPlayerEnter { get; private set; } = new UnityEvent();
    [field: SerializeField] public UnityEvent OnPlayerExit { get; private set; } = new UnityEvent();

    private Collider _triggerCollider;
    private bool _hasTriggered;
    private bool _hasExited;
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        EnsureTriggerCollider();
    }

    private void Reset()
    {
        EnsureTriggerCollider();
    }

    private void OnTriggerEnter(Collider other)
    {
        TryHandleTrigger(other, ref _hasTriggered, OnPlayerEnter);
    }

    private void OnTriggerExit(Collider other)
    {
        TryHandleTrigger(other, ref _hasExited, OnPlayerExit);
    }
    #endregion

    #region Public Methods
    public void ResetTrigger()
    {
        _hasTriggered = false;
        _hasExited = false;
    }
    #endregion

    #region Private Methods
    private void EnsureTriggerCollider()
    {
        if (_triggerCollider == null)
        {
            _triggerCollider = GetComponent<Collider>();
        }

        if (_triggerCollider != null)
        {
            _triggerCollider.isTrigger = true;
        }
    }

    private void TryHandleTrigger(Collider other, ref bool triggeredFlag, UnityEvent triggerEvent)
    {
        if ((TriggerOnce && triggeredFlag) || !IsPlayer(other))
        {
            return;
        }

        triggeredFlag = true;
        triggerEvent?.Invoke();
    }

    private static bool IsPlayer(Collider other)
    {
        return other.TryGetComponent(out Player _) || other.GetComponentInParent<Player>() != null;
    }
    #endregion
}
