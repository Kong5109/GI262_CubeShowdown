using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEffectArrowSO", menuName = "Weapon/Arrow/Effect Arrow")]
public class EffectArrowSO : ArrowSO
{
    #region Fields & Properties
    [field: Header("Effects")]
    [field: SerializeField] public List<Effect> Effects { get; private set; } = new List<Effect>();
    #endregion
}

