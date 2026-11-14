using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewArrowSO", menuName = "Weapon/Arrow Data")]
public class ArrowSO : ScriptableObject
{
    #region Fields & Properties
    [field: Header("Base")]
    [field: SerializeField] public float Speed { get; private set; } = 20f;
    [field: SerializeField] public float LifeTime { get; private set; } = 10f;
    [field: SerializeField] public int Damage { get; private set; } = 5;


    [field: Header("Pierce")]
    [field: SerializeField] public bool CanPierce { get; private set; }
    [field: SerializeField] public int MaxPierceCount { get; private set; }

    [field: Header("Spawn On Destroy")]
    [field: SerializeField] public GameObject SpawnPrefabOnDestroy { get; private set; }
    [field: SerializeField] public ArrowAtrribute ArrowAtrribute { get; private set; }
    [field: Header("Homing")]
    [field: SerializeField] public Homing Homing { get; private set; } = new Homing();

    [field: Header("Bouncing")]
    [field: SerializeField] public Bouncing Bouncing { get; private set; } = new Bouncing();

    [field: Header("Effects")]
    [field: SerializeField] public List<Effect> Effects { get; private set; } = new List<Effect>();
    #endregion
}
