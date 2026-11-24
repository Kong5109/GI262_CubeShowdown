using System.Collections.Generic;
using UnityEngine;

public class IceArea : MonoBehaviour
{
    #region Fields & Properties
    [field: SerializeField, Range(0.1f, 1f)] public float SlowMultiplier { get; private set; } = 0.5f;
    [field: SerializeField] public float Radius { get; private set; } = 3f;
    [field: SerializeField] public float Duration { get; private set; } = 5f;
    [field: SerializeField] public LayerMask EnemyLayer { get; private set; } = ~0;
    [field: SerializeField] public LayerMask GroundLayer { get; private set; } = DefaultGroundLayerMask;
    [field: SerializeField] public float GroundOffset { get; private set; } = 0.05f;
    [field: SerializeField] public float GroundCheckDistance { get; private set; } = 50f;

    private const int DefaultGroundLayerMask = 1;
    private const int MaxDetectedColliders = 64;
    private const int MaxGroundHits = 16;
    private const float MinRadius = 0f;
    private const float GroundRayStartOffset = 1f;
    private const float ExpiredTime = 0f;

    private readonly Collider[] _overlapResults = new Collider[MaxDetectedColliders];
    private readonly RaycastHit[] _groundHits = new RaycastHit[MaxGroundHits];
    private readonly HashSet<Enemy> _currentEnemies = new HashSet<Enemy>();
    private readonly HashSet<Enemy> _affectedEnemies = new HashSet<Enemy>();
    private readonly List<Enemy> _exitedEnemies = new List<Enemy>();
    private float _remainingDuration;
    #endregion

    #region Unity Lifecycle
    private void Start()
    {
        SnapToGround();
        _remainingDuration = Duration;
    }

    private void Update()
    {
        _remainingDuration -= Time.deltaTime;
        if (_remainingDuration <= ExpiredTime)
        {
            Destroy(gameObject);
            return;
        }

        UpdateSlowedEnemies();
    }

    private void OnDestroy()
    {
        ClearAllSlowModifiers();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, Radius);
    }
    #endregion

    #region Public Methods
    #endregion

    #region Private Methods
    private void SnapToGround()
    {
        Vector3 rayOrigin = transform.position + (Vector3.up * GroundRayStartOffset);
        int hitCount = Physics.RaycastNonAlloc(
            rayOrigin,
            Vector3.down,
            _groundHits,
            GroundCheckDistance + GroundRayStartOffset,
            GroundLayer,
            QueryTriggerInteraction.Ignore);

        float closestDistance = float.MaxValue;
        bool foundGround = false;
        float groundY = transform.position.y;

        for (int i = 0; i < hitCount; i++)
        {
            RaycastHit hit = _groundHits[i];
            if (hit.collider == null ||
                hit.collider.GetComponentInParent<Enemy>() != null ||
                hit.collider.GetComponentInParent<Player>() != null)
            {
                continue;
            }

            if (hit.distance < closestDistance)
            {
                closestDistance = hit.distance;
                groundY = hit.point.y;
                foundGround = true;
            }
        }

        if (foundGround)
        {
            Vector3 pos = transform.position;
            transform.position = new Vector3(pos.x, groundY + GroundOffset, pos.z);
        }

        transform.rotation = Quaternion.identity;
    }

    private void UpdateSlowedEnemies()
    {
        _currentEnemies.Clear();

        if (Radius > MinRadius)
        {
            int hitCount = Physics.OverlapSphereNonAlloc(transform.position, Radius, _overlapResults, EnemyLayer);
            for (int i = 0; i < hitCount; i++)
            {
                Collider col = _overlapResults[i];
                if (col == null)
                {
                    continue;
                }

                Enemy enemy = col.GetComponentInParent<Enemy>();
                if (enemy != null)
                {
                    _currentEnemies.Add(enemy);
                    if (_affectedEnemies.Add(enemy))
                    {
                        enemy.ApplySpeedModifier(this, SlowMultiplier);
                    }
                }
            }
        }

        _exitedEnemies.Clear();
        foreach (Enemy enemy in _affectedEnemies)
        {
            if (enemy == null || !_currentEnemies.Contains(enemy))
            {
                _exitedEnemies.Add(enemy);
            }
        }

        for (int i = 0; i < _exitedEnemies.Count; i++)
        {
            Enemy exitedEnemy = _exitedEnemies[i];
            _affectedEnemies.Remove(exitedEnemy);
            if (exitedEnemy != null)
            {
                exitedEnemy.RemoveSpeedModifier(this);
            }
        }
    }

    private void ClearAllSlowModifiers()
    {
        foreach (Enemy enemy in _affectedEnemies)
        {
            if (enemy != null)
            {
                enemy.RemoveSpeedModifier(this);
            }
        }

        _affectedEnemies.Clear();
    }
    #endregion
}

