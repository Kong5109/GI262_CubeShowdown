using UnityEngine;

[CreateAssetMenu(fileName = "NewArrowSO", menuName = "Weapon/Arrow/Normal Arrow")]
public class ArrowSO : ScriptableObject
{
    #region Fields & Properties
    [field: Header("Base")]
    [field: SerializeField] public float Speed { get; private set; } = 20f;
    [field: SerializeField] public float LifeTime { get; private set; } = 10f;
    [field: SerializeField] public int Damage { get; private set; } = 30;
    #endregion
}
