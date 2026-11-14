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

    [Header("Double Shot Attribute")]
    [field: SerializeField] public float distanceFormShootPoint { get; private set; } = 0.5f;

    [Header("Spread Shot Attribute")]
    [field: SerializeField] public int NumberOfArrow { get; private set; } = 3;
    [field: SerializeField] public float Angle { get; private set; } = 90f;

    private const float HalfAngleFactor = 0.5f;
    #endregion

    #region Unity Lifecycle
    #endregion

    #region Public Methods
    public void Shoot(GameObject arrow,Player player)
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

        player.CurrentMana -= ManaConsume;
    }
    #endregion

    #region Private Methods
    private void ShootSingleShot(GameObject arrow, Vector3 dir)
    {
        if (ShootPoint == null) return;

        SpawnArrow(arrow, ShootPoint.position, dir);
    }

    private void ShootDoubleShot(GameObject arrow, Vector3 dir)
    {
        if (arrow == null || ShootPoint == null) return;

        Vector3 rightAxis = Vector3.Cross(Vector3.up, dir).normalized;
        if (rightAxis == Vector3.zero)
        {
            rightAxis = ShootPoint.right;
        }

        Vector3 offset = rightAxis * distanceFormShootPoint;
        SpawnArrow(arrow, ShootPoint.position - offset, dir);
        SpawnArrow(arrow, ShootPoint.position + offset, dir);
    }

    private void ShootSpreadShot(GameObject arrow, Vector3 dir)
    {
        if (arrow == null || ShootPoint == null || NumberOfArrow <= 0) return;

        if (NumberOfArrow == 1)
        {
            SpawnArrow(arrow, ShootPoint.position, dir);
            return;
        }

        float startAngle = -Angle * HalfAngleFactor;
        float angleStep = Angle / (NumberOfArrow - 1);

        for (int i = 0; i < NumberOfArrow; i++)
        {
            float currentAngle = startAngle + (angleStep * i);
            Vector3 spreadDir = Quaternion.AngleAxis(currentAngle, Vector3.up) * dir;
            SpawnArrow(arrow, ShootPoint.position, spreadDir);
        }
    }

    private void SpawnArrow(GameObject arrow, Vector3 spawnPosition, Vector3 dir)
    {
        if (arrow == null || ShootPoint == null) return;

        Quaternion spawnRotation = dir != Vector3.zero
            ? Quaternion.LookRotation(dir)
            : ShootPoint.rotation;

        GameObject obj = Instantiate(arrow, spawnPosition, spawnRotation);

        if (obj.TryGetComponent(out Arrow arrowComp))
        {
            arrowComp.SetUp(Damage);
        }
    }
    #endregion
}
