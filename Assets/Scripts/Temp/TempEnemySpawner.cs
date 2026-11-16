using UnityEngine;

public class TempEnemySpawner : MonoBehaviour
{
    public GameObject Prefebs;
    void Start()
    {
        InvokeRepeating("Spawner", 3, 1);
    }
    private void Spawner()
    {
        Instantiate(Prefebs, transform.position, Quaternion.identity);
    }
}
