using UnityEngine;

[CreateAssetMenu(fileName = "NewHomingArrowSO", menuName = "Weapon/Arrow/Homing Arrow")]
public class HomingArrowSO : ArrowSO
{
    #region Fields & Properties
    [field: Header("Homing")]
    [field: SerializeField] public Homing Homing { get; private set; } = new Homing();
    #endregion
}

