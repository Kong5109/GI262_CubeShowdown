using System;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    #region Fields & Properties
    [field: Header("Wave Config")]
    [field: SerializeField] public int CurrentWave { get; private set; } = 1;
    [field: SerializeField] public WaveRule[] WaveRules { get; private set; }
    [field: SerializeField] public float SpawnInterval { get; private set; } = 0.5f;

    [field: Header("Spawn Points")]
    [field: SerializeField] public Transform[] SpawnPoints { get; private set; }

    public bool IsSpawning { get; private set; }
    public int RemainingEnemiesToSpawn => _enemyLeftToSpawn.Count;

    public event Action<int> OnWaveStarted;
    public event Action<GameObject> OnEnemySpawned;
    public event Action<int> OnWaveSpawnCompleted;

    private const float ReadyTimer = 0f;
    private const float MinSpawnInterval = 0f;
    private const float BoundsCenterFactor = 0.5f;
    private const float GizmoAreaHeight = 0.1f;
    private const int EmptyQueueCount = 0;

    private readonly Queue<GameObject> _enemyLeftToSpawn = new Queue<GameObject>();
    private float _spawnTimer;
    #endregion

    #region Unity Lifecycle
    private void Update()
    {
        if (!IsSpawning)
        {
            return;
        }

        _spawnTimer -= Time.deltaTime;
        if (_spawnTimer > ReadyTimer)
        {
            return;
        }

        SpawnNextEnemy();
    }

    private void OnDrawGizmosSelected()
    {
        if (!TryGetSpawnBounds(out Vector3 minBounds, out Vector3 maxBounds, out float spawnY, out _))
        {
            return;
        }

        Vector3 center = new Vector3(
            (minBounds.x + maxBounds.x) * BoundsCenterFactor,
            spawnY,
            (minBounds.z + maxBounds.z) * BoundsCenterFactor);
        Vector3 size = new Vector3(
            maxBounds.x - minBounds.x,
            GizmoAreaHeight,
            maxBounds.z - minBounds.z);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(center, size);
    }
    #endregion

    #region Public Methods
    [ContextMenu("Start Wave")]
    public void StartWave()
    {
        CurrentWave++;
        CalculateEnemyToSpawn();
        OnWaveStarted?.Invoke(CurrentWave);

        if (_enemyLeftToSpawn.Count == EmptyQueueCount)
        {
            IsSpawning = false;
            OnWaveSpawnCompleted?.Invoke(CurrentWave);
            return;
        }

        IsSpawning = true;
        _spawnTimer = ReadyTimer;
    }

    public void CalculateEnemyToSpawn()
    {
        if (WaveRules == null)
        {
            return;
        }

        for (int i = 0; i < WaveRules.Length; i++)
        {
            WaveRule rule = WaveRules[i];
            if (rule == null)
            {
                continue;
            }

            int spawnAmount = rule.GetSpawnAmount(CurrentWave);
            for (int j = 0; j < spawnAmount; j++)
            {
                _enemyLeftToSpawn.Enqueue(rule.EnemyPrefab);
            }
        }
    }
    #endregion

    #region Private Methods
    private void SpawnNextEnemy()
    {
        if (_enemyLeftToSpawn.Count == EmptyQueueCount)
        {
            FinishSpawningWave();
            return;
        }

        GameObject enemyPrefab = _enemyLeftToSpawn.Dequeue();
        if (enemyPrefab != null)
        {
            Vector3 spawnPosition = GetRandomSpawnPosition();
            Quaternion spawnRotation = GetSpawnRotation();
            GameObject spawnedEnemy = Instantiate(enemyPrefab, spawnPosition, spawnRotation);
            OnEnemySpawned?.Invoke(spawnedEnemy);
        }

        if (_enemyLeftToSpawn.Count == EmptyQueueCount)
        {
            FinishSpawningWave();
            return;
        }

        _spawnTimer = Mathf.Max(MinSpawnInterval, SpawnInterval);
    }

    private void FinishSpawningWave()
    {
        IsSpawning = false;
        OnWaveSpawnCompleted?.Invoke(CurrentWave);
    }

    private Vector3 GetRandomSpawnPosition()
    {
        if (!TryGetSpawnBounds(out Vector3 minBounds, out Vector3 maxBounds, out float spawnY, out _))
        {
            return transform.position;
        }

        float randomX = UnityEngine.Random.Range(minBounds.x, maxBounds.x);
        float randomZ = UnityEngine.Random.Range(minBounds.z, maxBounds.z);
        return new Vector3(randomX, spawnY, randomZ);
    }

    private Quaternion GetSpawnRotation()
    {
        return TryGetSpawnBounds(out _, out _, out _, out Quaternion spawnRotation)
            ? spawnRotation
            : transform.rotation;
    }

    private bool TryGetSpawnBounds(out Vector3 minBounds, out Vector3 maxBounds, out float spawnY, out Quaternion spawnRotation)
    {
        minBounds = Vector3.zero;
        maxBounds = Vector3.zero;
        spawnY = transform.position.y;
        spawnRotation = transform.rotation;

        if (SpawnPoints == null || SpawnPoints.Length == 0)
        {
            return false;
        }

        bool hasValidPoint = false;

        for (int i = 0; i < SpawnPoints.Length; i++)
        {
            Transform point = SpawnPoints[i];
            if (point == null)
            {
                continue;
            }

            Vector3 pos = point.position;
            if (!hasValidPoint)
            {
                minBounds = pos;
                maxBounds = pos;
                spawnY = pos.y;
                spawnRotation = point.rotation;
                hasValidPoint = true;
                continue;
            }

            minBounds = Vector3.Min(minBounds, pos);
            maxBounds = Vector3.Max(maxBounds, pos);
        }

        return hasValidPoint;
    }
    #endregion
}

