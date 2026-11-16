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
    [field: SerializeField] public BowSO BowSO { get; private set; }
    [field: SerializeField] public Transform ShootPoint { get; private set; }

    private const float HalfAngleFactor = 0.5f;
    #endregion

    #region Unity Lifecycle
    #endregion

    #region Public Methods
    public void SetUp(BowSO bowData)
    {
        BowSO = bowData;
    }

    public void Shoot(GameObject arrow)
    {
        if (BowSO == null || ShootPoint == null) return;

        switch (BowSO.ShootType)
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
        if (ShootPoint == null) return;

        SpawnArrow(arrow, ShootPoint.position, dir);
    }

    private void ShootDoubleShot(GameObject arrow, Vector3 dir)
    {
        if (arrow == null || ShootPoint == null || BowSO == null) return;

        Vector3 rightAxis = Vector3.Cross(Vector3.up, dir).normalized;
        if (rightAxis == Vector3.zero)
        {
            rightAxis = ShootPoint.right;
        }

        Vector3 offset = rightAxis * BowSO.distanceFormShootPoint;
        SpawnArrow(arrow, ShootPoint.position - offset, dir);
        SpawnArrow(arrow, ShootPoint.position + offset, dir);
    }

    private void ShootSpreadShot(GameObject arrow, Vector3 dir)
    {
        if (arrow == null || ShootPoint == null || BowSO == null || BowSO.NumberOfArrow <= 0) return;

        if (BowSO.NumberOfArrow == 1)
        {
            SpawnArrow(arrow, ShootPoint.position, dir);
            return;
        }

        float startAngle = -BowSO.Angle * HalfAngleFactor;
        float angleStep = BowSO.Angle / (BowSO.NumberOfArrow - 1);

        for (int i = 0; i < BowSO.NumberOfArrow; i++)
        {
            float currentAngle = startAngle + (angleStep * i);
            Vector3 spreadDir = Quaternion.AngleAxis(currentAngle, Vector3.up) * dir;
            SpawnArrow(arrow, ShootPoint.position, spreadDir);
        }
    }

    private void SpawnArrow(GameObject arrow, Vector3 spawnPosition, Vector3 dir)
    {
        if (arrow == null || ShootPoint == null || BowSO == null) return;

        Quaternion spawnRotation = dir != Vector3.zero
            ? Quaternion.LookRotation(dir)
            : ShootPoint.rotation;

        GameObject obj = Instantiate(arrow, spawnPosition, spawnRotation);

        if (obj.TryGetComponent(out Arrow arrowComp))
        {
            arrowComp.SetUp(BowSO.Damage);
        }
    }
    #endregion
}
