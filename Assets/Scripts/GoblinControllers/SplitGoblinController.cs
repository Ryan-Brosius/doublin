using UnityEngine;
using static InputManager;

public class SplitGoblinController : MonoBehaviour
{
    [Header("Control Settings")]
    [SerializeField] private PlayerID goblinPlayer;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField, Tooltip("How quickly the player accelerates")]
    private float acceleration = 10f;
    [SerializeField, Tooltip("How quickly the player decelerates")]
    private float deceleration = 10f;
    [SerializeField, Tooltip("How quickly the player rotates toward movement direction")]
    private float rotationSpeed = 10f;

    private Vector2 currentMove;
    private Vector3 velocity;
    private Vector3 moveDirection;

    LazyDependency<InputManager> _InputManager;
    LazyDependency<CameraManager> _CameraManager;

    #region Unity Functions
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
    }

    private void OnDisable()
    {
        // we NEED to disable the controls if the object gets disabled
        DisableGoblinControls();
    }

    private void FixedUpdate()
    {
        // Handle our player movement here
        HandleMovement();
        HandleRotation();
    }
    #endregion

    #region Input
    private void SetupGoblinControls()
    {
        switch (goblinPlayer)
        {
            case PlayerID.GrimoireGoblin:
                _InputManager.Value.OnGrimoireMove += HandleMove;
                break;
            case PlayerID.StaffGoblin:
                _InputManager.Value.OnStaffMove += HandleMove;
                break;
        }
    }

    private void DisableGoblinControls()
    {
        switch (goblinPlayer)
        {
            case PlayerID.GrimoireGoblin:
                _InputManager.Value.OnGrimoireMove -= HandleMove;
                break;
            case PlayerID.StaffGoblin:
                _InputManager.Value.OnStaffMove -= HandleMove;
                break;
        }
    }

    private void HandleMove(Vector2 move)
    {
        // Handling it through a function so we can remove it from the subscribe later
        currentMove = move;
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

        transform.position += velocity * Time.deltaTime;
    }

    private void HandleRotation()
    {
        if (currentMove.sqrMagnitude > 0.01f)
        {
            if (moveDirection.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
    }
    #endregion
}
