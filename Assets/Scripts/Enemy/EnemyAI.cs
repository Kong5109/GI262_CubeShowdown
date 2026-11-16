using System;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    #region Fields & Properties
    [field: SerializeField] public Vector2 MoveInput { get; private set; }
    [field: SerializeField] public Player Target { get; private set; }
    [field: SerializeField] public float TargetRefreshInterval { get; private set; } = 0.25f;
    [field: SerializeField] public float AttackTriggerRange { get; private set; } = 1.5f;

    public event Action OnAttacking;

    private float _refreshTimer;
    #endregion

    #region Unity Lifecycle
    private void Start()
    {
        UpdateTarget();
    }

    private void Update()
    {
        _refreshTimer -= Time.deltaTime;
        if (Target == null || !Target.gameObject.activeInHierarchy || _refreshTimer <= 0f)
        {
            _refreshTimer = TargetRefreshInterval;
            UpdateTarget();
        }
    }
    #endregion

    #region Public Methods
    public void SetTarget(Player target)
    {
        Target = target;
    }

    public void UpdateTarget()
    {
        Target = FindNearestPlayer();
    }
    #endregion

    #region Private Methods
    private Player FindNearestPlayer()
    {
        Player[] players = FindObjectsByType<Player>(FindObjectsSortMode.None);
        Player nearestPlayer = null;
        float closestSqrDistance = float.MaxValue;

        for (int i = 0; i < players.Length; i++)
        {
            Player player = players[i];
            if (player == null || !player.gameObject.activeInHierarchy)
            {
                continue;
            }

            float sqrDistance = (player.transform.position - transform.position).sqrMagnitude;
            if (sqrDistance < closestSqrDistance)
            {
                closestSqrDistance = sqrDistance;
                nearestPlayer = player;
            }
        }

        return nearestPlayer;
    }
    #endregion
}
