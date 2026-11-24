using System;
using UnityEngine;

[Serializable]
public class WaveRule
{
    #region Fields & Properties
    [field: SerializeField] public GameObject EnemyPrefab { get; private set; }
    [field: SerializeField] public int StartSpawnAmount { get; private set; } = 1;
    [field: SerializeField] public int IncreaseAmountPerWave { get; private set; } = 1;
    [field: SerializeField] public int MaxSpawnAmount { get; private set; } = 10;
    [field: SerializeField] public int StartSpawnAtWave { get; private set; } = 1;
    [field: SerializeField] public int EndSpawnAtWave { get; private set; } = 0;
    [field: SerializeField] public int SpawnEveryWave { get; private set; } = 1;

    private const int MinSpawnEveryWave = 1;
    private const int UnlimitedThreshold = 0;
    private const int NoSpawnAmount = 0;
    #endregion

    #region Public Methods
    public int GetSpawnAmount(int wave)
    {
        if (EnemyPrefab == null || wave < StartSpawnAtWave)
        {
            return NoSpawnAmount;
        }

        if (EndSpawnAtWave > UnlimitedThreshold && wave > EndSpawnAtWave)
        {
            return NoSpawnAmount;
        }

        int spacing = Mathf.Max(MinSpawnEveryWave, SpawnEveryWave);
        int waveOffset = wave - StartSpawnAtWave;
        if (waveOffset % spacing != 0)
        {
            return NoSpawnAmount;
        }

        int stepCount = waveOffset / spacing;
        int amount = StartSpawnAmount + (stepCount * IncreaseAmountPerWave);
        amount = Mathf.Max(NoSpawnAmount, amount);

        if (MaxSpawnAmount > UnlimitedThreshold)
        {
            amount = Mathf.Min(amount, MaxSpawnAmount);
        }

        return amount;
    }
    #endregion
}

