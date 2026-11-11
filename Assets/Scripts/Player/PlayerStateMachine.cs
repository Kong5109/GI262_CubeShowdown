using UnityEngine;

public class PlayerStateMachine : MonoBehaviour
{
    #region Fields & Properties
    [Header("Reference")]
    [field: SerializeField] public GameObject CharacterObj { get; private set; }
    [field: SerializeField] public Player Player { get; private set; }
    [field: SerializeField] public Animator Animator { get; private set; }
    [field: SerializeField] public CharacterController Controller { get; private set; }
    [field: SerializeField] public InputManager InputManager { get; private set; }
    [field: SerializeField] public Transform MainCameraTransform { get; private set; }

    //[field: SerializeField] public float MovementSpeed { get; private set; } = 5f;
    [field: SerializeField] public float RotationSpeed { get; private set; } = 12f;
    [field: SerializeField] public float AnimationBlendSpeed { get; private set; } = 10f;

    private PlayerBaseState state;
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

        if (Player == null)
        {
            Player = GetComponent<Player>();
        }
    }

    private void Start()
    {
        SwitchState(new PlayerNormalState(this));
    }

    private void Update()
    {
        this.state?.Tick(Time.deltaTime);
    }
    #endregion

    #region Public Methods
    public void SwitchState(PlayerBaseState state)
    {
        this.state?.Exit();
        this.state = state;
        this.state?.Enter();
    }

    public void Shoot()
    {
        Player.Shoot();
    }
    #endregion

    #region Private Methods

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, Player.TargetRange);
    }
    #endregion
}
