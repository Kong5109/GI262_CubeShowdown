using UnityEngine;

public class Enemy : MonoBehaviour
{
    [field: SerializeField] public int HP { get; private set; } = 100; 
    [field: SerializeField] public float Speed { get; private set; } = 2;

    private Player player;

    private void Start()
    {
        player = FindAnyObjectByType<Player>();
    }

    private void Update()
    {
        /*if (player == null) return;
        Vector3 dir = (player.transform.position - transform.position).normalized;
        transform.Translate(dir * Time.deltaTime);*/
    }

    public void TakeDamage(int damage)
    {
        HP -= damage;
        CheckDeath();
    }

    private void CheckDeath()
    {
        if (HP <= 0)
        {
            Destroy(gameObject);
        }
    }
}
