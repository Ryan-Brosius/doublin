using UnityEngine;
using static InputManager;

[RequireComponent(typeof(Rigidbody))]
public class CombinedGoblinController : MonoBehaviour
{
    [SerializeField] private LayerMask myLayer;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float deceleration = 10f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private LayerMask groundMask;

    [Header("Slope Settings")]
    [SerializeField] private float slopeCheckDistance = 1f;

    [Header("Throw Settings")]
    [SerializeField] private float throwForce = 7f;
    [SerializeField] private float throwUpVector = 1;

    [Header("Goblin Meshes")]
    [SerializeField] private GameObject grimoireGoblin;
    [SerializeField] private GameObject staffGoblin;
    [SerializeField] private Vector3 localTopPosition;
    [SerializeField] private Vector3 localBottomPosition;
    private Vector3 CurrentTopPosition => transform.position + localTopPosition;
    private Vector3 CurrentBottomPosition => transform.position + localBottomPosition;

    private Vector2 currentMove;
    private Vector3 velocity;
    private Vector3 moveDirection;

    private bool isOnSlope;

    private RaycastHit slopeHit;

    private Rigidbody rb;

    private GameObject topGoblinMesh;
    private GameObject bottomGoblinMesh;

    LazyDependency<InputManager> _InputManager;
    LazyDependency<CameraManager> _CameraManager;
    LazyDependency<GoblinStateManager> _GoblinStateManager;

    #region Unity Functions
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        SetupGoblinControls();

        _GoblinStateManager.Value.CurrentGoblinState.Subscribe(SetUpFakeGoblinMeshes);
    }

    private void OnEnable()
    {
        if (_InputManager.IsResolvable())
        {
            SetupGoblinControls();
            PositionInGoblinStack();
        }

        if (_GoblinStateManager.IsResolvable())
            _GoblinStateManager.Value.CurrentGoblinState.Subscribe(SetUpFakeGoblinMeshes);
    }

    private void OnDisable()
    {
        if (_InputManager.IsResolvable())
        {
            DisableGoblinControls();
        }
        currentMove = Vector2.zero;

        _GoblinStateManager.Value.CurrentGoblinState.Unsubscribe(SetUpFakeGoblinMeshes);
    }

    private void FixedUpdate()
    {
        HandleSlopeCheck();
        HandleMovement();
        HandleRotation();
    }
    #endregion

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, Vector3.down * slopeCheckDistance);
    }

    #region Input
    private void SetupGoblinControls()
    {
        _InputManager.Value.OnCombinedMove -= HandleMove;
        _InputManager.Value.OnCombinedSeperate -= HandleSeperate;
        _InputManager.Value.OnCombinedMove += HandleMove;
        _InputManager.Value.OnCombinedSeperate += HandleSeperate;
    }

    private void DisableGoblinControls()
    {
        _InputManager.Value.OnCombinedMove -= HandleMove;
        _InputManager.Value.OnCombinedSeperate -= HandleSeperate;
    }

    private void HandleMove(Vector2 move)
    {
        currentMove = move;
    }

    private void HandleSeperate()
    {
        // Grab the top goblin, we are about to throw him
        var topGoblin = _GoblinStateManager.Value.GetTopGoblin();
        var bottomGoblin = _GoblinStateManager.Value.GetBottomGoblin();
        var throwingAngle = (transform.forward + Vector3.up * throwUpVector).normalized;

        //RemoveOurGoblinChildren();
        _GoblinStateManager.Value.SeperateGoblins();

        if (topGoblin.TryGetComponent<SplitGoblinController>(out SplitGoblinController splitGoblinController))
        {
            splitGoblinController.TeleportToPosition(CurrentTopPosition);
        }
        topGoblin.transform.SetPositionAndRotation(
            CurrentTopPosition,
            transform.rotation
        );

        if (bottomGoblin.TryGetComponent<SplitGoblinController>(out SplitGoblinController splitGoblinController2))
        {
            splitGoblinController2.TeleportToPosition(CurrentBottomPosition);
        }
        bottomGoblin.transform.SetPositionAndRotation(
            CurrentBottomPosition,
            transform.rotation
        );

        // Throw goblin lol
        var goblinController = topGoblin.GetComponent<SplitGoblinController>();
        goblinController.ThrowGoblin(transform.forward, throwForce);
    }
    #endregion

    private void HandleMovement()
    {
        // Programming sins have been committed today
        // Mimicks split Im so lazy this is shit we're all going to hell for this

        Vector3 camForward = _CameraManager.Value.GetCameraFlatForwardTransform(_GoblinStateManager.Value.CurrentGoblinState.Value == GoblinState.StaffTop ? PlayerID.StaffGoblin : PlayerID.GrimoireGoblin);
        Vector3 camRight = new Vector3(camForward.z, 0f, -camForward.x);

        Vector3 targetDirection = camForward * currentMove.y + camRight * currentMove.x;
        
        if (isOnSlope)
        {
            targetDirection = Vector3.ProjectOnPlane(targetDirection, slopeHit.normal).normalized;
        }

        targetDirection = Vector3.ClampMagnitude(targetDirection, 1f);

        if (targetDirection.magnitude > 0.01f)
        {
            velocity = Vector3.Lerp(velocity, targetDirection * moveSpeed, acceleration * Time.fixedDeltaTime);
            moveDirection = targetDirection;
        }
        else
        {
            velocity = Vector3.Lerp(velocity, Vector3.zero, deceleration * Time.fixedDeltaTime);
        }

        Vector3 newPosition = rb.position + velocity * Time.fixedDeltaTime;
        rb.MovePosition(newPosition);
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

    private void HandleRotation()
    {
        if (currentMove.sqrMagnitude > 0.01f && moveDirection.sqrMagnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDirection);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRot, rotationSpeed * Time.fixedDeltaTime));
        }
    }

    private void PositionInGoblinStack()
    {
        transform.position = _GoblinStateManager.Value.GetBottomGoblin().transform.position;
    }

    private void SetUpFakeGoblinMeshes(GoblinState goblinState)
    {
        switch (goblinState)
        {
            case GoblinState.Separated:
                break;
            case GoblinState.StaffTop:
                staffGoblin.transform.localPosition = localTopPosition;
                grimoireGoblin.transform.localPosition = localBottomPosition;
                break;
            case GoblinState.BookTop:
                staffGoblin.transform.localPosition = localBottomPosition;
                grimoireGoblin.transform.localPosition = localTopPosition;
                break;
        }
    }
}
