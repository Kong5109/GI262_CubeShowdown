# Unity Development & Architecture Rules (GI262_CubeShowdown)

โปรเจกต์นี้มีมาตรฐานและข้อกำหนดการเขียนโค้ด Unity C# รวมถึงกระบวนการทำงานที่ต้องปฏิบัติตามอย่างเคร่งครัดดังนี้:

---

## 1. กระบวนการทำงาน (Workflow Requirement)
- **Implementation Plan First**: ก่อนทำการสร้าง แก้ไข หรือ Refactor โค้ดใดๆ ต้องสร้าง Implementation Plan Artifact (เปิด `RequestFeedback: true`) ให้ผู้ใช้อ่านและกด **Approve** ก่อนเสมอ ห้ามเริ่มเขียนโค้ดก่อนได้รับการอนุมัติ

---

## 2. สไตล์และหลักการเขียนโค้ด (Coding Principles & Style)
- **Concise & Necessary**: เขียนโค้ดให้สั้น กระชับ เอาเท่าที่จำเป็น ตรงประเด็น
- **No Hard-Coding**: ห้ามฮาร์ดโค้ดค่าคงที่, สตริง, Tag, Layer หรือ Scene Name ลงในลอจิกโดยตรง ให้กำหนดผ่าน Configuration, Const, หรือ Serialized Field/Property
- **No Magic Numbers**: ห้ามมีตัวเลขลอย (Magic Numbers) ทุกค่าต้องมีชื่อตัวแปรหรือค่าคงที่ระบุความหมายชัดเจน (`private const float ...`, `[field: SerializeField] public float ...`)
- **OOP & Patterns**: ยึดหลัก Encapsulation, Single Responsibility, และใช้ Game Design Patterns อย่างเหมาะสม
  - **Singleton**: ใช้ `MonoSingleton<T>` ที่สั้น ปลอดภัย และป้องกัน duplicate instance
  - **Observer**: ใช้ C# `event Action` / `event Action<T>` (เบา, ไม่ผูกมัด Inspector, และต้อง Unsubscribe เสมอ)

---

## 3. ลำดับโครงสร้าง Script ใน Unity (Script Layout Order)
ทุก C# Script ของ Unity (`MonoBehaviour`) ต้องจัดวางโครงสร้างและแบ่งโซนด้วย `#region` ตามลำดับต่อไปนี้:

```csharp
using System;
using UnityEngine;

public class ExampleController : MonoBehaviour
{
    #region Fields & Properties
    // 1. ตัวแปรและพรอพเพอร์ตี้ให้อยู่บนสุดเสมอ
    // เน้นทำเป็น Property เป็นหลัก (ใช้ auto-property หรือ serialized property)
    [field: SerializeField] public float MoveSpeed { get; private set; } = 5f;
    [field: SerializeField] public int MaxHealth { get; private set; } = 100;

    // Serialized Fields สำหรับ Component References หรือ Private Inspector Fields
    [SerializeField] private Rigidbody2D _rb;
    
    // Private Variables
    private int _currentHealth;
    #endregion

    #region Unity Lifecycle
    // 2. Unity Method ให้เรียงตามลำดับ Unity Execution Order เสมอ:
    // Awake -> OnEnable -> Reset -> Start -> FixedUpdate -> Update -> LateUpdate -> OnDisable -> OnDestroy
    private void Awake()
    {
    }

    private void OnEnable()
    {
        // Subscribe Events ที่นี่
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
        // Unsubscribe Events ที่นี่เสมอ
    }

    private void OnDestroy()
    {
    }
    #endregion

    #region Public Methods
    // 3.1 เมธอดสาธารณะสำหรับให้ภายนอกเรียกใช้งาน
    public void TakeDamage(int amount)
    {
        _currentHealth = Mathf.Max(0, _currentHealth - amount);
    }
    #endregion

    #region Private Methods
    // 3.2 เมธอดภายในสำหรับประมวลผลลอจิกย่อย
    private void ApplyMovement()
    {
    }
    #endregion
}
```

---

## 4. ข้อกำหนด Region
- ต้องมี 4 โซนหลักเสมอ:
  1. `#region Fields & Properties`
  2. `#region Unity Lifecycle`
  3. `#region Public Methods`
  4. `#region Private Methods`
- ห้ามสลับลำดับของ Region และห้ามใส่โค้ดนอก Region ที่กำหนด

