using UnityEngine;

public class EnemyStateMachine : MonoBehaviour
{
    [field: SerializeField] public GameObject CharacterObj { get; private set; }
    [field: SerializeField] public Animator Animator { get; private set; }
    [field: SerializeField] public Enemy Enemy { get; private set; }
    [field: SerializeField] public EnemyAI EnemyAI { get; private set; }
    [field: SerializeField] public CharacterController Controller { get; private set; }
    [field: SerializeField] public float RotationSpeed { get; private set; } = 12f;
    [field: SerializeField] public float AnimationBlendSpeed { get; private set; } = 10f;


    private EnemyBaseState state;

    private void Awake()
    {
        if (EnemyAI == null)
        {
            EnemyAI = GetComponent<EnemyAI>();
        }
    }

    private void Start()
    {
        SwitchState(new EnemyChasingState(this));
    }

    private void Update()
    {
        state?.Tick(Time.deltaTime);
    }

    public void SwitchState(EnemyBaseState state)
    {
        this.state?.Exit();
        this.state = state;
        this.state?.Enter();
    }
}
