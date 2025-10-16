using UnityEngine;

public class GrimoireGoblinController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    private Vector2 currentMove;
    LazyDependency<InputManager> _InputManager;

    private void Start()
    {
        _InputManager.Value.OnGrimoireMove += HandleMove;
    }

    private void OnEnable()
    {
        if (_InputManager.IsResolvable())
            _InputManager.Value.OnGrimoireMove += HandleMove;
    }

    private void OnDisable()
    {
        _InputManager.Value.OnGrimoireMove -= HandleMove;
    }

    private void HandleMove(Vector2 move)
    {
        currentMove = move;
    }

    private void FixedUpdate()
    {
        Vector3 move = new Vector3(currentMove.x, 0, currentMove.y);
        transform.Translate(move * moveSpeed * Time.deltaTime, Space.World);
    }
}
