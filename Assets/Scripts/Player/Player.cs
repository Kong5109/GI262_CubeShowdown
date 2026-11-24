using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region Fields & Properties
    [Header("Stat")]
    [field: SerializeField] public int MaxHP { get; private set; } = 100;
    [field: SerializeField] public int HPRegen { get; private set; } = 5;
    [field: SerializeField] public int Speed { get; private set; } = 8;
    [field: SerializeField] public int CurrentHP { get; set; }
    [field: SerializeField] public int Gold { get; private set; }

    [field: SerializeField] public WeaponHolder WeaponHolder { get; private set; }
    [field: SerializeField] public float TargetRange { get; private set; } = 7f;

    public event Action<int> OnGoldChanged;

    private const int MinGoldChange = 0;
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        if (WeaponHolder == null)
        {
            WeaponHolder = GetComponentInChildren<WeaponHolder>();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, TargetRange);
    }
    #endregion

    #region Public Methods
    public void Shoot()
    {
        WeaponHolder?.Shoot();
    }

    public void AddGold(int amount)
    {
        if (amount <= MinGoldChange)
        {
            return;
        }

        Gold += amount;
        OnGoldChanged?.Invoke(Gold);
    }

    public bool TrySpendGold(int amount)
    {
        if (amount < MinGoldChange || Gold < amount)
        {
            return false;
        }

        Gold -= amount;
        OnGoldChanged?.Invoke(Gold);
        return true;
    }
    #endregion

    #region Private Methods
    #endregion
}
