using UnityEngine;

[CreateAssetMenu(fileName = "NewBouncingArrowSO", menuName = "Weapon/Arrow/Bouncing Arrow")]
public class BouncingArrowSO : ArrowSO
{
    #region Fields & Properties
    [field: Header("Bouncing")]
    [field: SerializeField] public Bouncing Bouncing { get; private set; } = new Bouncing();
    #endregion
}

