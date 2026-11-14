using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Homing
{
    #region Fields & Properties
    [field: SerializeField] public float TurnSpeed { get; set; } = 360f;
    [field: SerializeField] public float HomingRange { get; set; } = 15f;
    [field: SerializeField] public LayerMask EnemyLayer { get; set; } = ~0;

    private const int MaxDetectedColliders = 32;

    private readonly Collider[] _overlapResults = new Collider[MaxDetectedColliders];
    #endregion

    #region Public Methods
    public Homing Clone()
    {
        return new Homing
        {
            TurnSpeed = TurnSpeed,
            HomingRange = HomingRange,
            EnemyLayer = EnemyLayer
        };
    }

    public static Vector3 CalculateDirection(Vector3 currentPosition, Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - currentPosition;
        if (direction.sqrMagnitude <= Mathf.Epsilon)
        {
            return Vector3.zero;
        }

        return direction.normalized;
    }

    public Quaternion CalculateRotation(
        Vector3 currentPosition,
        Quaternion currentRotation,
        Vector3 targetPosition,
        float deltaTime)
    {
        Vector3 direction = CalculateDirection(currentPosition, targetPosition);
        if (direction == Vector3.zero)
        {
            return currentRotation;
        }

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        return Quaternion.RotateTowards(currentRotation, targetRotation, TurnSpeed * deltaTime);
    }

    public Enemy FindNearestEnemy(Vector3 origin, HashSet<Enemy> ignoredEnemies = null)
    {
        if (HomingRange <= 0f)
        {
            return null;
        }

        int hitCount = Physics.OverlapSphereNonAlloc(origin, HomingRange, _overlapResults, EnemyLayer);
        Enemy nearestEnemy = null;
        float closestSqrDistance = float.MaxValue;

        for (int i = 0; i < hitCount; i++)
        {
            Collider col = _overlapResults[i];
            if (col == null || !col.TryGetComponent(out Enemy enemy))
            {
                continue;
            }

            if (ignoredEnemies != null && ignoredEnemies.Contains(enemy))
            {
                continue;
            }

            float sqrDistance = (enemy.transform.position - origin).sqrMagnitude;
            if (sqrDistance < closestSqrDistance)
            {
                closestSqrDistance = sqrDistance;
                nearestEnemy = enemy;
            }
        }

        return nearestEnemy;
    }
    #endregion
}
