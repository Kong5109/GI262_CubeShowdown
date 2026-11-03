using UnityEngine;

public abstract class PlayerBaseState
{
    protected PlayerStateMachine stateMachine;

    public PlayerBaseState (PlayerStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    public abstract void Enter();

    public abstract void Tick(float deltaTIme);
    public abstract void Exit();
}
