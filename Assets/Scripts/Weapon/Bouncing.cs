using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Bouncing
{
    #region Fields & Properties
    [field: SerializeField] public float BounceRange { get; set; } = 15f;
    [field: SerializeField] public int MaxBounceCount { get; set; } = 3;
    [field: SerializeField] public LayerMask EnemyLayer { get; set; } = ~0;

    private const int MaxDetectedColliders = 32;

    private readonly Collider[] _overlapResults = new Collider[MaxDetectedColliders];
    private Enemy _lastHitEnemy;
    private int _bounceCount;
    #endregion

    #region Public Methods
    public Bouncing Clone()
    {
        return new Bouncing
        {
            BounceRange = BounceRange,
            MaxBounceCount = MaxBounceCount,
            EnemyLayer = EnemyLayer
        };
    }

    public bool IsLastHit(Enemy enemy)
    {
        return enemy == _lastHitEnemy;
    }

    public bool TryBounce(Transform projectileTransform, Enemy currentEnemy, HashSet<Enemy> hitEnemies)
    {
        _lastHitEnemy = currentEnemy;

        if (MaxBounceCount > 0 && _bounceCount >= MaxBounceCount)
        {
            return false;
        }

        Enemy nextTarget = FindNextTarget(projectileTransform.position, currentEnemy, hitEnemies);
        if (nextTarget == null)
        {
            return false;
        }

        _bounceCount++;
        projectileTransform.rotation = CalculateBounceRotation(
            projectileTransform.position,
            projectileTransform.rotation,
            nextTarget.transform.position);
        return true;
    }

    public Enemy FindNextTarget(Vector3 origin, Enemy currentEnemy, HashSet<Enemy> hitEnemies)
    {
        if (BounceRange <= 0f)
        {
            return null;
        }

        int hitCount = Physics.OverlapSphereNonAlloc(origin, BounceRange, _overlapResults, EnemyLayer);
        Enemy nearestUnhit = null;
        Enemy nearestHit = null;
        float closestUnhitSqrDistance = float.MaxValue;
        float closestHitSqrDistance = float.MaxValue;

        for (int i = 0; i < hitCount; i++)
        {
            Collider col = _overlapResults[i];
            if (col == null || !col.TryGetComponent(out Enemy enemy) || enemy == currentEnemy)
            {
                continue;
            }

            float sqrDistance = (enemy.transform.position - origin).sqrMagnitude;
            bool isAlreadyHit = hitEnemies != null && hitEnemies.Contains(enemy);

            if (!isAlreadyHit)
            {
                if (sqrDistance < closestUnhitSqrDistance)
                {
                    closestUnhitSqrDistance = sqrDistance;
                    nearestUnhit = enemy;
                }
            }
            else if (sqrDistance < closestHitSqrDistance)
            {
                closestHitSqrDistance = sqrDistance;
                nearestHit = enemy;
            }
        }

        return nearestUnhit != null ? nearestUnhit : nearestHit;
    }

    public Quaternion CalculateBounceRotation(Vector3 currentPosition, Quaternion currentRotation, Vector3 targetPosition)
    {
        Vector3 flatTargetPosition = new Vector3(targetPosition.x, currentPosition.y, targetPosition.z);
        Vector3 direction = Homing.CalculateDirection(currentPosition, flatTargetPosition);
        if (direction == Vector3.zero)
        {
            return currentRotation;
        }

        return Quaternion.LookRotation(direction);
    }
    #endregion
}
