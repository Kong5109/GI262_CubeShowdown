using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Arrow : MonoBehaviour
{
    #region Fields & Properties
    [field: SerializeField] public float Speed { get; private set; } = 20f;
    [field: SerializeField] public float LifeTime { get; private set; } = 10f;
    [field: SerializeField] public int Damage { get; private set; } = 5;
    #endregion

    #region Unity Lifecycle
    private void Start()
    {
        Destroy(gameObject, LifeTime);
    }

    private void Update()
    {
        transform.position += transform.forward * (Speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider target)
    {
        if (target.TryGetComponent(out Enemy enemy))
        {
            enemy.TakeDamage(Damage);
            Destroy(gameObject);
        }
    }
    #endregion

    #region Public Methods
    public void SetUp(int damage)
    {
        Damage = damage;
    }
    #endregion

    #region Private Methods
    #endregion
}
