using UnityEngine;

[CreateAssetMenu(fileName = "NewPierceArrowSO", menuName = "Weapon/Arrow/Pierce Arrow")]
public class PierceArrowSO : ArrowSO
{
    #region Fields & Properties
    [field: Header("Pierce")]
    [field: SerializeField] public int MaxPierceCount { get; private set; } = 3;
    #endregion
}

