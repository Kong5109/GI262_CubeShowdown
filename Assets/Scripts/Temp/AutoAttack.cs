using UnityEngine;

public class AutoAttack : MonoBehaviour
{
    private InputManager inputManager;
    public float time = 1f;
    private void Start()
    {
        inputManager = GetComponent<InputManager>();
        InvokeRepeating("Attack", time, time);
    }

    private void Attack()
    {
        inputManager?.OnAttack();
    }
}
