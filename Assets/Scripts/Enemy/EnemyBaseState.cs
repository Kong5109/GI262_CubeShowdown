using UnityEngine;

public abstract class EnemyBaseState
{
    protected EnemyStateMachine stateMachine;

    public EnemyBaseState(EnemyStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    public abstract void Enter();

    public abstract void Tick(float deltaTIme);
    public abstract void Exit();
}
