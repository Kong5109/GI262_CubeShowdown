using System;
using System.Collections.Generic;
using UnityEngine;

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
        if (ArrowSO is HomingArrowSO homingSO && homingSO.Homing != null)
        {
            _runtimeHoming = homingSO.Homing.Clone();
        }

        if (ArrowSO is BouncingArrowSO bouncingSO && bouncingSO.Bouncing != null)
        {
            _runtimeBouncing = bouncingSO.Bouncing.Clone();
        }
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

        if (_runtimeHoming != null)
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

        if (_runtimeBouncing != null)
        {
            HandleBouncingHit(enemy);
            return;
        }

        HandleStandardHit(enemy);
    }

    private void OnDrawGizmos()
    {
        if (ArrowSO is not HomingArrowSO homingSO || homingSO.Homing == null || homingSO.Homing.HomingRange <= 0f)
        {
            return;
        }

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, homingSO.Homing.HomingRange);
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

        if (ArrowSO is PierceArrowSO pierceSO)
        {
            if (pierceSO.MaxPierceCount > 0 && _hitCount >= pierceSO.MaxPierceCount)
            {
                DestroyArrow();
            }

            return;
        }

        DestroyArrow();
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
        if (ArrowSO is not EffectArrowSO effectSO || effectSO.Effects == null || effectSO.Effects.Count == 0)
        {
            return;
        }

        foreach (Effect effect in effectSO.Effects)
        {
            if (effect == null)
            {
                continue;
            }

            Type effectType = effect.GetType();
            if (enemy.GetComponentInChildren(effectType) is Effect existingEffect)
            {
                existingEffect.RefreshDuration();
                continue;
            }

            Effect spawnedEffect = Instantiate(effect, enemy.transform);
            spawnedEffect.transform.localPosition = Vector3.zero;
            spawnedEffect.transform.localRotation = Quaternion.identity;
        }
    }

    private void DestroyArrow()
    {
        if (_isDestroyed)
        {
            return;
        }

        _isDestroyed = true;

        if (_hasDealtDamage && ArrowSO is SpawnObjectArrowSO spawnSO && spawnSO.SpawnPrefabOnDestroy != null)
        {
            Instantiate(spawnSO.SpawnPrefabOnDestroy, transform.position, transform.rotation);
        }

        Destroy(gameObject);
    }
    #endregion
}
