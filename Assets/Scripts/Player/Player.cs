using UnityEngine;

public class Player : MonoBehaviour
{
    #region Fields & Properties
    [field: SerializeField] public float TargetRange { get; private set; } = 7f;
    #endregion

    #region Unity Lifecycle
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, TargetRange);
    }
    #endregion

    #region Public Methods
    #endregion

    #region Private Methods
    #endregion
}
