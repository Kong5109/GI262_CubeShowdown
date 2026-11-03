---
name: game-architect
description: >-
  Expert Unity Game Architect skill for designing, implementing, and refactoring Unity C# architectures using OOP principles, Singleton, Observer pattern, strict Unity execution order, and concise #region layouts. Always creates an Implementation Plan for user approval before writing code.
---

# Game Architect Skill (Unity C#)

This skill equips the agent with professional Game Architecture capabilities for Unity, enforcing object-oriented design (OOP), essential game design patterns (Singleton, Observer), clean code practices, and strict script layout conventions.

---

## 1. Mandatory Workflow: Implementation Plan First

**Rule**: Before modifying existing code or creating new scripts, you **MUST** formulate an Implementation Plan artifact and wait for the user to approve it.

1. Create a markdown artifact in the artifact directory (`implementation_plan.md`) with `ArtifactMetadata`:
   - `UserFacing: true`
   - `RequestFeedback: true`
2. Describe the architecture, design patterns, file changes, and verification steps clearly.
3. Stop calling tools and prompt the user to review and click **Approve / Proceed**.
4. Only proceed with implementation after approval is received.

---

## 2. Coding Principles & Clean Code Standards

- **Concise & Necessary**: Write concise, minimal code. Avoid unnecessary boilerplate, over-engineering, or speculative features.
- **No Hard-Coding**: Never hard-code gameplay values, scene names, tags, or layer names directly in logic. Expose them via serialized properties, ScriptableObjects, or dedicated constants.
- **No Magic Numbers**: Any numeric literal with semantic meaning must be defined as a `const`, `static readonly`, or serialized configuration property.
- **OOP Principles**:
  - **Encapsulation**: Expose state via read-only properties (`public int Value { get; private set; }`), never via public fields.
  - **Single Responsibility (SRP)**: Keep classes focused on one gameplay system (e.g., separate Input, Movement, and Health).
  - **Loose Coupling**: Use events for cross-system communication.

---

## 3. Game Design Patterns Standard

### A. Singleton Pattern (`MonoSingleton<T>`)
- Use a reusable generic base class `MonoSingleton<T>` for global managers (e.g., `GameManager`, `AudioManager`).
- Ensure null-safety, automatic persistence check (`DontDestroyOnLoad`), and destruction of duplicate instances.
- Keep singletons minimal; do not turn everything into a singleton.

### B. Observer Pattern (C# `event Action`)
- Use standard C# `event Action` / `event Action<T>` for lightweight, decoupled communication.
- **Lifecycle Rule**: Always subscribe in `OnEnable()` and unsubscribe in `OnDisable()` to prevent memory leaks and missing reference exceptions.
- Avoid tight coupling where observers directly call concrete manager methods unless through well-defined singleton interfaces.

---

## 4. Unity Script Layout & #region Specifications

Every Unity `MonoBehaviour` script must follow this exact order and partition into 4 standard `#region` blocks:

```csharp
using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Fields & Properties
    // 1. Properties as primary API
    [field: SerializeField] public float MoveSpeed { get; private set; } = 5f;
    [field: SerializeField] public int MaxHealth { get; private set; } = 100;

    // 2. Serialized Fields for Inspector references
    [SerializeField] private Rigidbody2D _rb;

    // 3. Private / State Fields
    private int _currentHealth;
    #endregion

    #region Unity Lifecycle
    // Ordered strictly by Unity Execution Order:
    // Awake -> OnEnable -> Reset -> Start -> FixedUpdate -> Update -> LateUpdate -> OnDisable -> OnDestroy
    private void Awake()
    {
    }

    private void OnEnable()
    {
        // Event subscriptions
    }

    private void Start()
    {
    }

    private void FixedUpdate()
    {
    }

    private void Update()
    {
    }

    private void LateUpdate()
    {
    }

    private void OnDisable()
    {
        // Event unsubscriptions
    }

    private void OnDestroy()
    {
    }
    #endregion

    #region Public Methods
    // Public API and external operations
    public void RestoreHealth(int amount)
    {
        _currentHealth = Mathf.Min(MaxHealth, _currentHealth + amount);
    }
    #endregion

    #region Private Methods
    // Internal helper methods and computations
    private void HandleMovement()
    {
    }
    #endregion
}
```

---

## 5. Reference Templates

Detailed, ready-to-use template implementations are available in:
- [Reference Templates](./references/templates.md)
  - `MonoSingleton<T>` Generic Implementation
  - Event-Driven Observer Pattern Example
  - Full-featured Component Architecture Template

