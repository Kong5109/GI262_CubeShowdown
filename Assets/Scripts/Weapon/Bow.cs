using UnityEngine;

public enum ShootType
{
    SingleShot,
    DoubleShot,
    SpreadShot
}

public class Bow : MonoBehaviour
{
    #region Fields & Properties
    [field: Header("Attribute")]
    [field: SerializeField] public int ManaConsume { get; private set; } = 20;
    [field: SerializeField] public int Damage { get; private set; } = 30;
    [field: SerializeField] public float Cooldown { get; private set; } = 1f;
    [field: SerializeField] public ShootType ShootType { get; private set; } = ShootType.SingleShot;
    [field: SerializeField] public Transform ShootPoint { get; private set; }
    #endregion

    #region Unity Lifecycle
    #endregion

    #region Public Methods
    public void Shoot(GameObject arrow)
    {
        switch (ShootType)
        {
            case ShootType.SingleShot:
                ShootSingleShot(arrow, ShootPoint.forward);
                break;
            case ShootType.DoubleShot:
                ShootDoubleShot(arrow, ShootPoint.forward);
                break;
            case ShootType.SpreadShot:
                ShootSpreadShot(arrow, ShootPoint.forward);
                break;
            default:
                ShootSingleShot(arrow, ShootPoint.forward);
                break;
        }
    }
    #endregion

    #region Private Methods
    private void ShootSingleShot(GameObject arrow, Vector3 dir)
    {
        if (arrow == null || ShootPoint == null) return;

        Quaternion spawnRotation = dir != Vector3.zero 
            ? Quaternion.LookRotation(dir) 
            : ShootPoint.rotation;

        GameObject obj = Instantiate(arrow, ShootPoint.position, spawnRotation);

        if (obj.TryGetComponent(out Arrow arrowComp))
        {
            arrowComp.SetUp(Damage);
        }
    }

    private void ShootDoubleShot(GameObject arrow, Vector3 dir)
    {
        Debug.Log($"Shoot Double Shot {arrow.name} + {dir}");
    }

    private void ShootSpreadShot(GameObject arrow, Vector3 dir)
    {
        Debug.Log($"Shoot Spread Shot {arrow.name} + {dir}");
    }
    #endregion
}
