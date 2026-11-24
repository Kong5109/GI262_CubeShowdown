using System.Collections.Generic;
using UnityEngine;

public class ExplosionArea : MonoBehaviour
{
    #region Fields & Properties
    [field: SerializeField] public int Damage { get; private set; } = 40;
    [field: SerializeField] public float Radius { get; private set; } = 3.5f;
    [field: SerializeField] public float LifeTime { get; private set; } = 0.6f;
    [field: SerializeField] public LayerMask EnemyLayer { get; private set; } = ~0;

    private const int MaxDetectedColliders = 64;
    private const int MinDamage = 0;
    private const float MinRadius = 0f;
    private const float MinLifeTime = 0f;

    private readonly Collider[] _overlapResults = new Collider[MaxDetectedColliders];
    private readonly HashSet<Enemy> _damagedEnemies = new HashSet<Enemy>();
    #endregion

    #region Unity Lifecycle
    private void Start()
    {
        DealExplosionDamage();
        Destroy(gameObject, Mathf.Max(MinLifeTime, LifeTime));
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, Radius);
    }
    #endregion

    #region Public Methods
    #endregion

    #region Private Methods
    private void DealExplosionDamage()
    {
        if (Radius <= MinRadius || Damage <= MinDamage)
        {
            return;
        }

        _damagedEnemies.Clear();
        int hitCount = Physics.OverlapSphereNonAlloc(transform.position, Radius, _overlapResults, EnemyLayer);

        for (int i = 0; i < hitCount; i++)
        {
            Collider col = _overlapResults[i];
            if (col == null)
            {
                continue;
            }

            Enemy enemy = col.GetComponentInParent<Enemy>();
            if (enemy != null && _damagedEnemies.Add(enemy))
            {
                enemy.TakeDamage(Damage);
            }
        }
    }
    #endregion
}

