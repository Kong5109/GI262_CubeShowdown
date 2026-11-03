# Game Architect Reference Templates (Unity C#)

This reference document contains concrete, minimal, and production-ready implementations of the architectural patterns required by the `game-architect` skill.

---

## 1. Generic MonoSingleton Base Class (`MonoSingleton<T>`)

A concise, robust singleton base class for Unity `MonoBehaviour` singletons.

```csharp
using UnityEngine;

public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    #region Fields & Properties
    public static T Instance { get; private set; }

    [field: SerializeField] public bool IsPersistent { get; private set; } = false;
    #endregion

    #region Unity Lifecycle
    protected virtual void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this as T;

        if (IsPersistent)
        {
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        }
    }

    protected virtual void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
    #endregion
}
```

### Usage Example:
```csharp
public class GameManager : MonoSingleton<GameManager>
{
    #region Fields & Properties
    [field: SerializeField] public int Score { get; private set; }
    #endregion

    #region Public Methods
    public void AddScore(int amount)
    {
        Score += amount;
    }
    #endregion
}
```

---

## 2. Observer Pattern (C# `event Action`)

Decoupled event broadcasting and subscription without inspector binding overhead.

### Publisher Component (`PlayerHealth.cs`):
```csharp
using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    #region Fields & Properties
    // Events
    public static event Action<int, int> OnHealthChanged; // (current, max)
    public static event Action OnPlayerDied;

    [field: SerializeField] public int MaxHealth { get; private set; } = 100;
    
    private int _currentHealth;
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        _currentHealth = MaxHealth;
    }

    private void Start()
    {
        OnHealthChanged?.Invoke(_currentHealth, MaxHealth);
    }
    #endregion

    #region Public Methods
    public void TakeDamage(int damage)
    {
        if (_currentHealth <= 0) return;

        _currentHealth = Mathf.Max(0, _currentHealth - damage);
        OnHealthChanged?.Invoke(_currentHealth, MaxHealth);

        if (_currentHealth == 0)
        {
            OnPlayerDied?.Invoke();
        }
    }
    #endregion
}
```

### Subscriber Component (`HealthUI.cs`):
```csharp
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    #region Fields & Properties
    [SerializeField] private Slider _healthSlider;
    #endregion

    #region Unity Lifecycle
    private void OnEnable()
    {
        PlayerHealth.OnHealthChanged += HandleHealthChanged;
        PlayerHealth.OnPlayerDied += HandlePlayerDied;
    }

    private void OnDisable()
    {
        PlayerHealth.OnHealthChanged -= HandleHealthChanged;
        PlayerHealth.OnPlayerDied -= HandlePlayerDied;
    }
    #endregion

    #region Private Methods
    private void HandleHealthChanged(int current, int max)
    {
        if (_healthSlider != null)
        {
            _healthSlider.value = (float)current / max;
        }
    }

    private void HandlePlayerDied()
    {
        // Handle UI effects on death
    }
    #endregion
}
```

---

## 3. Strict Script Layout Checklist

When reviewing or writing any script:

- [ ] **No Magic Numbers**: Are all numbers named constants or serialized fields?
- [ ] **Properties First**: Are state variables exposed via properties rather than public fields?
- [ ] **Regions Present & In Order**:
  1. `#region Fields & Properties`
  2. `#region Unity Lifecycle`
  3. `#region Public Methods`
  4. `#region Private Methods`
- [ ] **Execution Order Correct**: Are lifecycle methods strictly ordered (`Awake` -> `OnEnable` -> `Reset` -> `Start` -> `FixedUpdate` -> `Update` -> `LateUpdate` -> `OnDisable` -> `OnDestroy`)?
- [ ] **Event Cleanup**: Are all registered events properly unregistered in `OnDisable()` or `OnDestroy()`?

