using UnityEngine;

public class SpringTween : MonoBehaviour
{
    [Header("Spring Settings")]
    [SerializeField] private float halfLife = 0.075f;
    [SerializeField] private float frequency = 18f;
    [SerializeField] private float angularDisplacement = 2f;
    [SerializeField] private float linearDisplacement = 0.05f;

    private Vector3 startingLocalPosition;
    private Vector3 springPosition;
    private Vector3 springVelocity;

    private void Awake()
    {
        startingLocalPosition = transform.localPosition;
        springPosition = transform.position;
        springVelocity = Vector3.zero;
    }

    public void Initialize()
    {
        springPosition = transform.position;
        springVelocity = Vector3.zero;
    }

    private void Update()
    {
        transform.localPosition = Vector3.zero;

        Spring(ref springPosition, ref springVelocity, transform.position, halfLife, frequency, Time.deltaTime);

        var localPosition = springPosition - transform.position;
        var springHeight = Vector3.Dot(localPosition, Vector3.up);

        transform.localEulerAngles = new Vector3(-springHeight * angularDisplacement, 0f, 0f);
        transform.localPosition = localPosition * linearDisplacement + startingLocalPosition;
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        Gizmos.color = Color.green;

        Gizmos.DrawLine(transform.position, springPosition + transform.localPosition);
        Gizmos.DrawSphere(springPosition + transform.localPosition, 0.1f);
    }

    private static void Spring(ref Vector3 current, ref Vector3 velocity, Vector3 target, float halfLife, float frequency, float timeStep)
    {
        var dampingRatio = -Mathf.Log(0.5f) / (frequency / halfLife);
        var f = 1.0f + 2.0f * timeStep * dampingRatio * frequency;
        var oo = frequency * frequency;
        var hoo = timeStep * oo;
        var hhoo = timeStep * hoo;
        var detInv = 1.0f / (f + hhoo);
        var detX = f * current + timeStep * velocity + hhoo * target;
        var detV = velocity + hoo * (target - current);
        current = detX * detInv;
        velocity = detV * detInv;
    }
}
