using UnityEngine;

public class Player : MonoBehaviour
{
    #region Fields & Properties
    [Header("Stat")]
    [field: SerializeField] public int MaxHP { get; private set; } = 100;
    [field: SerializeField] public int HPRegen { get; private set; } = 5;
    [field: SerializeField] public int MaxMana { get; private set; } = 200;
    [field: SerializeField] public int ManaRegen { get; private set; } = 10;
    [field: SerializeField] public int Speed { get; private set; } = 8;

    [field: SerializeField] public Bow Bow { get; private set; }
    [field: SerializeField] public GameObject ArrowPrefabs { get; private set; }
    [field: SerializeField] public float TargetRange { get; private set; } = 7f;
    #endregion

    #region Unity Lifecycle
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, TargetRange);
    }
    #endregion

    #region Public Methods
    public void Shoot()
    {
        Bow.Shoot(ArrowPrefabs);
    }
    #endregion

    #region Private Methods
    #endregion
}
