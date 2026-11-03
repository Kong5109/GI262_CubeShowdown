using UnityEngine;

public class PlayerStateMachine : MonoBehaviour
{
    [field: SerializeField] public Animator Animator { get; private set; }
    [field: SerializeField] public PlayerBaseState State { get; private set; }
    [field:SerializeField] public InputManager InputManager { get; private set; }
    public void Update()
    {
        State?.Tick(Time.deltaTime);
    }

    public void SwitchState(PlayerBaseState state)
    {
        State?.Exit();
        State = state;
        State?.Enter();
    }
    #region temp
    private void Start()
    {
        SwitchState(new PlayerWalkingState(this));
    }

    #endregion
}
