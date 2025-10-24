using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using static InputManager;

[RequireComponent(typeof(Rigidbody))]
public class SplitGoblinController : MonoBehaviour
{
    [Header("Control Settings")]
    [SerializeField] private PlayerID goblinPlayer;

    [Header("Movement Settings")]
    [SerializeField]
    private float moveSpeed = 5f;
    [SerializeField]
    [Tooltip("How quickly the player accelerates")]
    private float acceleration = 10f;
    [SerializeField]
    [Tooltip("How quickly the player decelerates")]
    private float deceleration = 10f;
    [SerializeField]
    [Tooltip("How quickly the player rotates toward movement direction")]
    private float rotationSpeed = 10f;

    [Header("Slope Settings")]
    [SerializeField] private float slopeCheckDistance = 1f;

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private float gravity = -9.8f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.25f;
    [SerializeField] private LayerMask groundMask;

    [Header("Combine Settings")]
    [Tooltip("Empty transform placed above the goblin’s head for detecting jump-on-top events")]
    [SerializeField] private Transform combineCheck;
    [SerializeField] private float combineCheckRadius = 0.5f;
    [SerializeField] private LayerMask combineMask;

    private Vector2 currentMove;
    private Vector3 velocity;
    private Vector3 moveDirection;

    private RaycastHit slopeHit;

    private Rigidbody rb;

    private bool isGrounded;
    private bool isOnSlope;
    public bool isInFreeFall = false;
    private bool combineCheckEnabled = true;

    LazyDependency<InputManager> _InputManager;
    LazyDependency<CameraManager> _CameraManager;
    LazyDependency<GoblinStateManager> _GoblinStateManager;

    #region Unity Functions
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    private void Start()
    {
        // Set up all controls for the player
        SetupGoblinControls();
    }

    private void OnEnable()
    {
        // Enable controls again when we enable
        if (_InputManager.IsResolvable())
            SetupGoblinControls();

        StartCoroutine(disableCombineCheck(1f));
        currentMove = Vector2.zero;
    }

    private void OnDisable()
    {
        // we NEED to disable the controls if the object gets disabled
        DisableGoblinControls();
    }

    private void FixedUpdate()
    {
        // Handle our player movement here
        HandleGroundCheck();
        if (!isInFreeFall)
        {
            HandleSlopeCheck();
            HandleMovement();
            HandleRotation();
        }
        HandleCombineCheck();
    }

    private void OnCollisionStay(Collision collision)
    {
        if (isInFreeFall && combineCheckEnabled)
        {
            if (((1 << collision.gameObject.layer) & groundMask) != 0)
            {
                isInFreeFall = false;
            }
        }
    }
    #endregion

    #region Gizmos
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        if (combineCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(combineCheck.position, combineCheckRadius);
        }

        Gizmos.DrawRay(transform.position, Vector3.down * slopeCheckDistance);
    }
    #endregion

    #region Input
    private void SetupGoblinControls()
    {
        switch (goblinPlayer)
        {
            case PlayerID.GrimoireGoblin:
                _InputManager.Value.OnGrimoireMove -= HandleMove;
                _InputManager.Value.OnGrimoireJump -= HandleJump;
                _InputManager.Value.OnGrimoireMove += HandleMove;
                _InputManager.Value.OnGrimoireJump += HandleJump;
                break;
            case PlayerID.StaffGoblin:
                _InputManager.Value.OnStaffMove -= HandleMove;
                _InputManager.Value.OnStaffJump -= HandleJump;
                _InputManager.Value.OnStaffMove += HandleMove;
                _InputManager.Value.OnStaffJump += HandleJump;
                break;
        }
    }

    private void DisableGoblinControls()
    {
        switch (goblinPlayer)
        {
            case PlayerID.GrimoireGoblin:
                _InputManager.Value.OnGrimoireMove -= HandleMove;
                _InputManager.Value.OnGrimoireJump -= HandleJump;
                break;
            case PlayerID.StaffGoblin:
                _InputManager.Value.OnStaffMove -= HandleMove;
                _InputManager.Value.OnStaffJump -= HandleJump;
                break;
        }
    }

    private void HandleMove(Vector2 move)
    {
        // Handling it through a function so we can remove it from the subscribe later
        currentMove = move;
    }

    private void HandleJump()
    {
        if (isGrounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
        }

        //if (_GoblinStateManager.Value.DistanceBetweenGoblins() <= combineRange)
        //{
        //    _GoblinStateManager.Value.CombineGoblins(goblinPlayer == PlayerID.GrimoireGoblin ? GoblinState.BookTop : GoblinState.StaffTop);
        //}
    }
    #endregion

    #region Movement
    private void HandleMovement()
    {
        // Grab the forward camera flattened on the XZ plane
        Vector3 camForward = _CameraManager.Value.GetCameraFlatForwardTransform(goblinPlayer);
        // Calculate our vector perpendicular of the XZ plane, gives us the strafe for left/right
        Vector3 camRight = new Vector3(camForward.z, 0f, -camForward.x);

        // The direction we want to move... duh
        Vector3 targetDirection = camForward * currentMove.y + camRight * currentMove.x;

        // If on slope project our target to be the slope
        if (isOnSlope)
        {
            targetDirection = Vector3.ProjectOnPlane(targetDirection, slopeHit.normal).normalized;
        }

        targetDirection = Vector3.ClampMagnitude(targetDirection, 1f);

        // If our direction magnitude is above 0, move the player
        // Otherwise start decelerating the player
        if (targetDirection.magnitude > 0.01f)
        {
            velocity = Vector3.Lerp(velocity, targetDirection * moveSpeed, acceleration * Time.deltaTime);
            moveDirection = targetDirection;
        }
        else
        {
            velocity = Vector3.Lerp(velocity, Vector3.zero, deceleration * Time.deltaTime);
        }

        Vector3 newPosition = rb.position + velocity * Time.fixedDeltaTime;
        rb.MovePosition(newPosition);
    }

    private void HandleRotation()
    {
        if (currentMove.sqrMagnitude > 0.01f && moveDirection.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            Quaternion smoothedRotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
            rb.MoveRotation(smoothedRotation);
        }
    }

    private void HandleGroundCheck()
    {
        if (groundCheck == null)
        {
            return;
        }

        Collider[] hits = Physics.OverlapSphere(groundCheck.position, groundCheckRadius, groundMask);

        // So you dont check collision with yourself :')
        isGrounded = false;
        foreach (Collider hit in hits)
        {
            if (hit.gameObject == gameObject)
                continue;
            if (hit.transform.IsChildOf(transform))
                continue;

            isGrounded = true;
            break;
        }
    }

    private void HandleSlopeCheck()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, slopeCheckDistance, groundMask))
        {
            isOnSlope = slopeHit.normal != Vector3.up;
        }
        else
        {
            isOnSlope = false;
        }
    }
    #endregion

    private void HandleCombineCheck()
    {
        if (combineCheck == null)
            return;
        if (!isGrounded)
            return;
        if (!combineCheckEnabled)
            return;

        Collider[] hits = Physics.OverlapSphere(combineCheck.position, combineCheckRadius, combineMask);

        foreach (Collider hit in hits)
        {
            if (hit.gameObject == gameObject)
                continue;
            if (hit.transform.IsChildOf(transform))
                continue;

            var otherGoblin = hit.GetComponent<SplitGoblinController>();
            if (otherGoblin != null)
            {
                _GoblinStateManager.Value.CombineGoblins(goblinPlayer == PlayerID.GrimoireGoblin ? GoblinState.StaffTop : GoblinState.BookTop);
                break;
            }
        }
    }

    public void ThrowGoblin(Vector3 direction, float force)
    {
        StartCoroutine(ThrowRoutine(direction, force));
    }

    private IEnumerator ThrowRoutine(Vector3 direction, float force)
    {
        combineCheckEnabled = false;

        Rigidbody rb = GetComponent<Rigidbody>();

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.AddForce(direction.normalized * force, ForceMode.VelocityChange);
        rb.AddTorque(Random.insideUnitSphere * 3f, ForceMode.VelocityChange);

        isInFreeFall = true;
        yield return new WaitForSeconds(0.2f);
        combineCheckEnabled = true;
    }

    public IEnumerator disableCombineCheck(float time)
    {
        combineCheckEnabled = false;
        yield return new WaitForSeconds(time);
        combineCheckEnabled = true;
    }

    public void TeleportToPosition(Vector3 newPosition)
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.position = newPosition;

        transform.position = newPosition;

        velocity = Vector3.zero;
        currentMove = Vector2.zero;
    }

    public void ApplyForce(Vector3 direction, float magnitude, ForceMode mode = ForceMode.VelocityChange)
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.AddForce(direction.normalized * magnitude, mode);
    }
}
