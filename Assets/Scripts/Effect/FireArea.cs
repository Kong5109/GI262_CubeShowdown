using System.Collections.Generic;
using UnityEngine;

public class FireArea : MonoBehaviour
{
    #region Fields & Properties
    [field: SerializeField] public int DamagePerTick { get; private set; } = 10;
    [field: SerializeField] public float TickInterval { get; private set; } = 1f;
    [field: SerializeField] public float Radius { get; private set; } = 3f;
    [field: SerializeField] public float Duration { get; private set; } = 5f;
    [field: SerializeField] public LayerMask EnemyLayer { get; private set; } = ~0;
    [field: SerializeField] public LayerMask GroundLayer { get; private set; } = DefaultGroundLayerMask;
    [field: SerializeField] public float GroundOffset { get; private set; } = 0.05f;
    [field: SerializeField] public float GroundCheckDistance { get; private set; } = 50f;

    private const int DefaultGroundLayerMask = 1;
    private const int MaxDetectedColliders = 64;
    private const int MaxGroundHits = 16;
    private const int MinDamage = 0;
    private const float MinRadius = 0f;
    private const float MinTickInterval = 0.05f;
    private const float GroundRayStartOffset = 1f;
    private const float ReadyTimer = 0f;
    private const float ExpiredTime = 0f;

    private readonly Collider[] _overlapResults = new Collider[MaxDetectedColliders];
    private readonly RaycastHit[] _groundHits = new RaycastHit[MaxGroundHits];
    private readonly HashSet<Enemy> _damagedThisTick = new HashSet<Enemy>();
    private float _remainingDuration;
    private float _tickTimer;
    #endregion

    #region Unity Lifecycle
    private void Start()
    {
        SnapToGround();
        _remainingDuration = Duration;
        _tickTimer = ReadyTimer;
    }

    private void Update()
    {
        _remainingDuration -= Time.deltaTime;
        if (_remainingDuration <= ExpiredTime)
        {
            Destroy(gameObject);
            return;
        }

        _tickTimer -= Time.deltaTime;
        if (_tickTimer > ReadyTimer)
        {
            return;
        }

        ApplyFireTick();
        _tickTimer = Mathf.Max(MinTickInterval, TickInterval);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.45f, 0f, 1f);
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

    private void ApplyFireTick()
    {
        if (Radius <= MinRadius || DamagePerTick <= MinDamage)
        {
            return;
        }

        _damagedThisTick.Clear();
        int hitCount = Physics.OverlapSphereNonAlloc(transform.position, Radius, _overlapResults, EnemyLayer);

        for (int i = 0; i < hitCount; i++)
        {
            Collider col = _overlapResults[i];
            if (col == null)
            {
                continue;
            }

            Enemy enemy = col.GetComponentInParent<Enemy>();
            if (enemy != null && _damagedThisTick.Add(enemy))
            {
                enemy.TakeDamage(DamagePerTick);
            }
        }
    }
    #endregion
}

