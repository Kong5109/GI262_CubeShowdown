using UnityEngine;

[CreateAssetMenu(fileName = "NewBowSO", menuName = "Weapon/Bow Data")]
public class BowSO : ScriptableObject
{
    #region Fields & Properties
    [field: Header("Attribute")]
    [field: SerializeField] public int ManaConsume { get; private set; } = 20;
    [field: SerializeField] public int Damage { get; private set; } = 30;
    [field: SerializeField] public float Cooldown { get; private set; } = 1f;
    [field: SerializeField] public ShootType ShootType { get; private set; } = ShootType.SingleShot;

    [field: Header("Double Shot Attribute")]
    [field: SerializeField] public float distanceFormShootPoint { get; private set; } = 0.5f;

    [field: Header("Spread Shot Attribute")]
    [field: SerializeField] public int NumberOfArrow { get; private set; } = 3;
    [field: SerializeField] public float Angle { get; private set; } = 90f;
    #endregion
}
