using UnityEngine;

public class EnemyStateMachine : MonoBehaviour
{
    #region Fields & Properties
    [field: SerializeField] public GameObject CharacterObj { get; private set; }
    [field: SerializeField] public Animator Animator { get; private set; }
    [field: SerializeField] public Enemy Enemy { get; private set; }
    [field: SerializeField] public CharacterController Controller { get; private set; }
    [field: SerializeField] public float RotationSpeed { get; private set; } = 12f;
    [field: SerializeField] public float AnimationBlendSpeed { get; private set; } = 10f;

    private const float PausedAnimatorSpeed = 0f;
    private const float NormalAnimatorSpeed = 1f;

    private EnemyBaseState _state;
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        if (Enemy == null)
        {
            Enemy = GetComponent<Enemy>();
        }
    }

    private void Start()
    {
        SwitchState(new EnemyChasingState(this));
    }

    private void Update()
    {
        bool isStunned = Enemy != null && Enemy.IsStunned;
        if (Animator != null)
        {
            Animator.speed = isStunned ? PausedAnimatorSpeed : NormalAnimatorSpeed;
        }

        if (isStunned)
        {
            return;
        }

        _state?.Tick(Time.deltaTime);
    }
    #endregion

    #region Public Methods
    public void SwitchState(EnemyBaseState state)
    {
        _state?.Exit();
        _state = state;
        _state?.Enter();
    }
    #endregion

    #region Private Methods
    #endregion
}
