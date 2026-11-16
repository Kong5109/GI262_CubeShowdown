using UnityEngine;

public class WeaponHolder : MonoBehaviour
{
    #region Fields & Properties
    [field: Header("Data")]
    [field: SerializeField] public BowSO BowData { get; private set; }
    [field: SerializeField] public GameObject ArrowPrefab { get; private set; }

    [field: Header("Reference")]
    [field: SerializeField] public Bow Bow { get; private set; }
    [field: SerializeField] public Player Player { get; private set; }
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        if (Bow != null && BowData != null)
        {
            Bow.SetUp(BowData);
        }
    }
    #endregion

    #region Public Methods
    public void Shoot()
    {
        if (Bow == null) return;

        if (BowData != null)
        {
            Bow.SetUp(BowData);
        }

        Bow.Shoot(ArrowPrefab);
    }
    #endregion

    #region Private Methods
    #endregion
}
