using UnityEngine;

public class StaffGoblinController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    private Vector2 currentMove;
    LazyDependency<InputManager> _InputManager;

    private void Start()
    {
        _InputManager.Value.OnStaffMove += HandleMove;
    }

    private void OnEnable()
    {
        if (_InputManager.IsResolvable())
            _InputManager.Value.OnStaffMove += HandleMove;
    }

    private void OnDisable()
    {
        _InputManager.Value.OnStaffMove -= HandleMove;
    }

    private void HandleMove(Vector2 move)
    {
        currentMove = move;
    }

    private void FixedUpdate()
    {
        Debug.Log(_InputManager.IsResolvable());

        Vector3 move = new Vector3(currentMove.x, 0, currentMove.y);
        transform.Translate(move * moveSpeed * Time.deltaTime, Space.World);
    }
}
