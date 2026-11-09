using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CharacterPhysics : MonoBehaviour
{
    #region Fields & Properties
    private const float JumpGravityFactor = -2f;
    private const float MinImpactThresholdSqr = 0.01f;

    [field: SerializeField] public float Gravity { get; private set; } = -9.81f;
    [field: SerializeField] public float GroundedPull { get; private set; } = -2f;
    [field: SerializeField] public float Drag { get; private set; } = 5f;

    public bool IsGrounded => _controller != null && _controller.isGrounded;

    [SerializeField] private CharacterController _controller;

    private Vector3 _verticalVelocity;
    private Vector3 _impactVelocity;
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        if (_controller == null)
        {
            _controller = GetComponent<CharacterController>();
        }
    }

    private void Update()
    {
        ApplyGravity();
        ApplyImpact();
        ApplyMovement();
    }
    #endregion

    #region Public Methods
    public void Jump(float jumpHeight)
    {
        _verticalVelocity.y = Mathf.Sqrt(jumpHeight * JumpGravityFactor * Gravity);
    }

    public void AddImpact(Vector3 direction, float force)
    {
        direction.Normalize();
        if (direction.y < 0f)
        {
            direction.y = -direction.y;
        }

        _impactVelocity += direction * force;
    }
    #endregion

    #region Private Methods
    private void ApplyGravity()
    {
        if (IsGrounded && _verticalVelocity.y < 0f)
        {
            _verticalVelocity.y = GroundedPull;
        }

        _verticalVelocity.y += Gravity * Time.deltaTime;
    }

    private void ApplyImpact()
    {
        if (_impactVelocity.sqrMagnitude > MinImpactThresholdSqr)
        {
            _impactVelocity = Vector3.Lerp(_impactVelocity, Vector3.zero, Drag * Time.deltaTime);
        }
        else
        {
            _impactVelocity = Vector3.zero;
        }
    }

    private void ApplyMovement()
    {
        Vector3 totalPhysicsMove = _verticalVelocity + _impactVelocity;
        _controller.Move(totalPhysicsMove * Time.deltaTime);
    }
    #endregion
}
