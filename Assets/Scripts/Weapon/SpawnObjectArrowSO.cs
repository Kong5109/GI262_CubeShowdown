using UnityEngine;

[CreateAssetMenu(fileName = "NewSpawnObjectArrowSO", menuName = "Weapon/Arrow/Spawn Object Arrow")]
public class SpawnObjectArrowSO : ArrowSO
{
    #region Fields & Properties
    [field: Header("Spawn On Destroy")]
    [field: SerializeField] public GameObject SpawnPrefabOnDestroy { get; private set; }
    #endregion
}

