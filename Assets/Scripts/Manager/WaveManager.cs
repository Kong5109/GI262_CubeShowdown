using System;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    #region Fields & Properties
    [field: Header("Wave Config")]
    [field: SerializeField] public int CurrentWave { get; private set; }
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
            Transform spawnPoint = GetSpawnPoint();
            GameObject spawnedEnemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
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

    private Transform GetSpawnPoint()
    {
        if (SpawnPoints == null || SpawnPoints.Length == 0)
        {
            return transform;
        }

        Transform selectedPoint = SpawnPoints[UnityEngine.Random.Range(0, SpawnPoints.Length)];
        return selectedPoint != null ? selectedPoint : transform;
    }
    #endregion
}

