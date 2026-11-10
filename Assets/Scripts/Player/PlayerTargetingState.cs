using UnityEngine;

public class PlayerTargetingState : PlayerBaseState
{
    #region Fields & Properties
    private const float DefaultTargetRange = 7f;
    #endregion

    #region Public Methods
    public PlayerTargetingState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        Enemy nearestEnemy = FindNearestEnemy();
        stateMachine?.SwitchState(new PlayerAttackState(stateMachine, nearestEnemy));
    }

    public override void Exit()
    {
    }

    public override void Tick(float deltaTIme)
    {
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Performs an OverlapSphere check around the player to locate and return the closest Enemy.
    /// </summary>
    private Enemy FindNearestEnemy()
    {
        if (stateMachine == null) return null;

        float targetRange = stateMachine.Player != null ? stateMachine.Player.TargetRange : DefaultTargetRange;
        Vector3 playerPos = stateMachine.transform.position;
        Collider[] colliders = Physics.OverlapSphere(playerPos, targetRange);

        Enemy nearestEnemy = null;
        float minDistanceSqr = Mathf.Infinity;

        for (int i = 0; i < colliders.Length; i++)
        {
            Enemy enemy = colliders[i].GetComponentInParent<Enemy>();
            if (enemy == null) continue;

            float distanceSqr = (enemy.transform.position - playerPos).sqrMagnitude;
            if (distanceSqr < minDistanceSqr)
            {
                minDistanceSqr = distanceSqr;
                nearestEnemy = enemy;
            }
        }

        return nearestEnemy;
    }
    #endregion
}
