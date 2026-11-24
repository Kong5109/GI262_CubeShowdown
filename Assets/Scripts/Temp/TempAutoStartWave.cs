using UnityEngine;

public class TempAutoStartWave : MonoBehaviour
{
    #region Fields & Properties
    [field: SerializeField] public WaveManager WaveManager { get; private set; }
    [field: SerializeField] public float WaveInterval { get; private set; } = 3f;
    [field: SerializeField] public bool StartImmediately { get; private set; } = true;

    private const float ReadyTimer = 0f;
    private const float MinWaveInterval = 0.01f;

    private float _waveTimer;
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        if (WaveManager == null)
        {
            WaveManager = GetComponent<WaveManager>();
        }
    }

    private void Start()
    {
        _waveTimer = StartImmediately ? ReadyTimer : Mathf.Max(MinWaveInterval, WaveInterval);
    }

    private void Update()
    {
        if (WaveManager == null)
        {
            return;
        }

        _waveTimer -= Time.deltaTime;
        if (_waveTimer > ReadyTimer)
        {
            return;
        }

        TriggerWave();
    }
    #endregion

    #region Public Methods
    #endregion

    #region Private Methods
    private void TriggerWave()
    {
        WaveManager.StartWave();
        _waveTimer = Mathf.Max(MinWaveInterval, WaveInterval);
    }
    #endregion
}
