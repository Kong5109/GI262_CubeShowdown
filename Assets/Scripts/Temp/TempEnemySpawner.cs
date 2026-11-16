using UnityEngine;

#warning For Test Delete This if you finish
public class TempEnemySpawner : MonoBehaviour
{
    public GameObject Prefebs;
    public float delayTime = 3;
    public float repeatTime = 5;
    void Start()
    {
        InvokeRepeating("Spawner", delayTime, repeatTime);
    }
    private void Spawner()
    {
        Instantiate(Prefebs, transform.position, Quaternion.identity);
    }
}
