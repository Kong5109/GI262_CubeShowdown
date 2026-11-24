using System;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    #region Fields & Properties
    [field: Header("Stats")]
    [field: SerializeField] public int HP { get; private set; } = 100;
    [field: SerializeField] public float Speed { get; private set; } = 2f;

    [field: Header("AI")]
    [field: SerializeField] public Vector2 MoveInput { get; private set; }
    [field: SerializeField] public Player Target { get; private set; }
    [field: SerializeField] public float TargetRefreshInterval { get; private set; } = 0.25f;
    [field: SerializeField] public float AttackTriggerRange { get; private set; } = 1.5f;

    public bool IsStunned => _stunSources.Count > 0;
    public float EffectiveSpeed => IsStunned ? ZeroSpeed : Speed * _speedMultiplier;

    public event Action OnAttacking;

    private const float DefaultSpeedMultiplier = 1f;
    private const float MinSpeedMultiplier = 0f;
    private const float ZeroSpeed = 0f;
    private const float ReadyTimer = 0f;
    private const int DeathHPThreshold = 0;

    private readonly Dictionary<object, float> _speedModifiers = new Dictionary<object, float>();
    private readonly HashSet<object> _stunSources = new HashSet<object>();
    private float _speedMultiplier = DefaultSpeedMultiplier;
    private float _refreshTimer;
    #endregion

    #region Unity Lifecycle
    private void Start()
    {
        UpdateTarget();
    }

    private void Update()
    {
        _refreshTimer -= Time.deltaTime;
        if (Target == null || !Target.gameObject.activeInHierarchy || _refreshTimer <= ReadyTimer)
        {
            _refreshTimer = TargetRefreshInterval;
            UpdateTarget();
        }
    }
    #endregion

    #region Public Methods
    public void TakeDamage(int damage)
    {
        HP -= damage;
        CheckDeath();
    }

    public void SetTarget(Player target)
    {
        Target = target;
    }

    public void UpdateTarget()
    {
        Target = FindNearestPlayer();
    }

    public void ApplySpeedModifier(object source, float multiplier)
    {
        if (source == null)
        {
            return;
        }

        _speedModifiers[source] = Mathf.Max(MinSpeedMultiplier, multiplier);
        RecalculateSpeedMultiplier();
    }

    public void RemoveSpeedModifier(object source)
    {
        if (source == null || !_speedModifiers.Remove(source))
        {
            return;
        }

        RecalculateSpeedMultiplier();
    }

    public void SetStunned(object source, bool isStunned)
    {
        if (source == null)
        {
            return;
        }

        if (isStunned)
        {
            _stunSources.Add(source);
            return;
        }

        _stunSources.Remove(source);
    }
    #endregion

    #region Private Methods
    private Player FindNearestPlayer()
    {
        Player[] players = FindObjectsByType<Player>(FindObjectsSortMode.None);
        Player nearestPlayer = null;
        float closestSqrDistance = float.MaxValue;

        for (int i = 0; i < players.Length; i++)
        {
            Player player = players[i];
            if (player == null || !player.gameObject.activeInHierarchy)
            {
                continue;
            }

            float sqrDistance = (player.transform.position - transform.position).sqrMagnitude;
            if (sqrDistance < closestSqrDistance)
            {
                closestSqrDistance = sqrDistance;
                nearestPlayer = player;
            }
        }

        return nearestPlayer;
    }

    private void RecalculateSpeedMultiplier()
    {
        float lowestMultiplier = DefaultSpeedMultiplier;
        foreach (float multiplier in _speedModifiers.Values)
        {
            lowestMultiplier = Mathf.Min(lowestMultiplier, multiplier);
        }

        _speedMultiplier = lowestMultiplier;
    }

    private void CheckDeath()
    {
        if (HP <= DeathHPThreshold)
        {
            Destroy(gameObject);
        }
    }
    #endregion
}
