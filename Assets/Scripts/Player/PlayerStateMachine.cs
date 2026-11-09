using UnityEngine;

public class PlayerStateMachine : MonoBehaviour
{
    #region Fields & Properties
    [field: SerializeField] public GameObject CharacterObj { get; private set; }
    [field: SerializeField] public Animator Animator { get; private set; }
    [field: SerializeField] public CharacterController Controller { get; private set; }
    [field: SerializeField] public PlayerBaseState State { get; private set; }
    [field: SerializeField] public InputManager InputManager { get; private set; }
    [field: SerializeField] public Transform MainCameraTransform { get; private set; }
    [field: SerializeField] public float MovementSpeed { get; private set; } = 5f;
    [field: SerializeField] public float RotationSpeed { get; private set; } = 12f;
    [field: SerializeField] public float AnimationBlendSpeed { get; private set; } = 10f;
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        if (MainCameraTransform == null && Camera.main != null)
        {
            MainCameraTransform = Camera.main.transform;
        }

        if (Controller == null)
        {
            Controller = GetComponent<CharacterController>();
        }
    }

    private void Start()
    {
        SwitchState(new PlayerWalkingState(this));
    }

    private void Update()
    {
        State?.Tick(Time.deltaTime);
    }
    #endregion

    #region Public Methods
    public void SwitchState(PlayerBaseState state)
    {
        State?.Exit();
        State = state;
        State?.Enter();
    }
    #endregion

    #region Private Methods
    #endregion
}
