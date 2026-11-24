using System;
using System.Collections.Generic;
using UnityEngine;

public enum ArrowAtrribute
{
    normal,
    homing,
    bouncing
}

[RequireComponent(typeof(Collider))]
public class Arrow : MonoBehaviour
{
    #region Fields & Properties
    [field: SerializeField] public ArrowSO ArrowSO { get; private set; }

    private readonly HashSet<Enemy> _hitEnemies = new HashSet<Enemy>();
    private Homing _runtimeHoming;
    private Bouncing _runtimeBouncing;
    private float _remainingLifeTime;
    private int _damage;
    private int _hitCount;
    private bool _hasDealtDamage;
    private bool _isDestroyed;
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        if (ArrowSO == null)
        {
            return;
        }

        _runtimeHoming = ArrowSO.Homing != null ? ArrowSO.Homing.Clone() : null;
        _runtimeBouncing = ArrowSO.Bouncing != null ? ArrowSO.Bouncing.Clone() : null;
    }

    private void Start()
    {
        if (ArrowSO == null)
        {
            Destroy(gameObject);
            return;
        }
        _damage = ArrowSO.Damage;
        _remainingLifeTime = ArrowSO.LifeTime;
    }

    private void Update()
    {
        if (ArrowSO == null)
        {
            return;
        }

        if (ArrowSO.ArrowAtrribute == ArrowAtrribute.homing)
        {
            ApplyHoming();
        }

        transform.position += transform.forward * (ArrowSO.Speed * Time.deltaTime);

        _remainingLifeTime -= Time.deltaTime;
        if (_remainingLifeTime <= 0f)
        {
            DestroyArrow();
        }
    }

    private void OnTriggerEnter(Collider target)
    {
        if (_isDestroyed || ArrowSO == null || !target.TryGetComponent(out Enemy enemy))
        {
            return;
        }

        if (ArrowSO.ArrowAtrribute == ArrowAtrribute.bouncing)
        {
            HandleBouncingHit(enemy);
            return;
        }

        HandleStandardHit(enemy);
    }

    private void OnDrawGizmos()
    {
        if (ArrowSO == null || ArrowSO.ArrowAtrribute != ArrowAtrribute.homing || ArrowSO.Homing == null || ArrowSO.Homing.HomingRange <= 0f)
        {
            return;
        }

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, ArrowSO.Homing.HomingRange);
    }
    #endregion

    #region Public Methods
    #endregion

    #region Private Methods
    private void ApplyHoming()
    {
        if (_runtimeHoming == null)
        {
            return;
        }

        Enemy nearestEnemy = _runtimeHoming.FindNearestEnemy(transform.position, _hitEnemies);
        if (nearestEnemy == null)
        {
            return;
        }

        transform.rotation = _runtimeHoming.CalculateRotation(
            transform.position,
            transform.rotation,
            nearestEnemy.transform.position,
            Time.deltaTime);
    }

    private void HandleBouncingHit(Enemy enemy)
    {
        if (_runtimeBouncing == null || _runtimeBouncing.IsLastHit(enemy))
        {
            return;
        }

        ApplyDamage(enemy);

        if (!_runtimeBouncing.TryBounce(transform, enemy, _hitEnemies))
        {
            DestroyArrow();
        }
    }

    private void HandleStandardHit(Enemy enemy)
    {
        if (_hitEnemies.Contains(enemy))
        {
            return;
        }

        ApplyDamage(enemy);

        if (!ArrowSO.CanPierce || (ArrowSO.MaxPierceCount > 0 && _hitCount >= ArrowSO.MaxPierceCount))
        {
            DestroyArrow();
        }
    }

    private void ApplyDamage(Enemy enemy)
    {
        _hitEnemies.Add(enemy);
        enemy.TakeDamage(_damage);
        ApplyEffect(enemy);
        _hasDealtDamage = true;
        _hitCount++;
    }

    private void ApplyEffect(Enemy enemy)
    {
        if (ArrowSO == null || ArrowSO.Effects == null || ArrowSO.Effects.Count == 0)
        {
            return;
        }

        foreach (Effect effect in ArrowSO.Effects)
        {
            if (effect == null)
            {
                continue;
            }

            Type effectType = effect.GetType();
            if (!enemy.TryGetComponent(effectType, out _))
            {
                enemy.gameObject.AddComponent(effectType);
            }
        }
    }

    private void DestroyArrow()
    {
        if (_isDestroyed)
        {
            return;
        }

        _isDestroyed = true;

        if (_hasDealtDamage && ArrowSO != null && ArrowSO.SpawnPrefabOnDestroy != null)
        {
            Instantiate(ArrowSO.SpawnPrefabOnDestroy, transform.position, transform.rotation);
        }

        Destroy(gameObject);
    }
    #endregion
}
