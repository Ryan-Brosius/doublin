using UnityEngine;

[CreateAssetMenu(fileName = "AimingSettings", menuName = "Combat/Aiming Settings")]
public class AimingSettings : ScriptableObject
{
    [Header("Detection Settings")]
    public float maxRange = 50f;
    [Range(0f, 45f)]
    public float aimConeAngle = 15f;
    public LayerMask targetLayers;
    public LayerMask obstructionLayer;

    [Header("Lock Behavior")]
    public float switchTargetCooldown = 0.25f;
    public float reticleSpinSpeed = 180f;

    [Header("Performance")]
    public int maxTargets = 64; // Overlapping buffer size

    [Header("Debug")]
    public bool showDebugGizmos = false;
}

public class AimingManager : SingletonMonobehavior<AimingManager>
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private AimingSettings settings;

    private Transform currentTarget;
    private float lastSwitchTime = 0f;
    private readonly Collider[] targetBuffer = new Collider[64];

    public Transform CurrentTarget => currentTarget;

    private void Update()
    {
        // Lets go baby run the check
        if (Time.time - lastSwitchTime > settings.switchTargetCooldown)
            FindTarget();
    }

    private void FindTarget()
    {
        Vector3 camPos = playerCamera.transform.position;
        Vector3 camForward = playerCamera.transform.forward;

        int count = Physics.OverlapSphereNonAlloc(
            camPos,
            settings.maxRange,
            targetBuffer,
            settings.targetLayers
        );

        Transform bestTarget = null;
        float bestScore = float.MaxValue;

        float cosThreshold = Mathf.Cos(settings.aimConeAngle * Mathf.Deg2Rad);

        for (int i = 0; i < count; i++)
        {
            Collider hit = targetBuffer[i];
            // Add component check here later

            Vector3 dirToTarget = (hit.transform.position - camPos);
            float distance = dirToTarget.magnitude;
            Vector3 dirNormalized = (dirToTarget / distance).normalized;

            float dot = Vector3.Dot(camForward, dirNormalized);
            if (dot <= 0f || dot < cosThreshold)
                continue;

            // Check line of sight is possible
            if (Physics.Raycast(camPos, dirNormalized, out RaycastHit losHit, distance, settings.obstructionLayer))
                continue;

            // Get the score
            float anglePenalty = 1f - dot;
            float score = distance + anglePenalty * 5f;

            if (score < bestScore)
            {
                bestScore = score;
                bestTarget = hit.transform;
            }
        }

        if (bestTarget != currentTarget)
        {
            currentTarget = bestTarget;
        }
        lastSwitchTime = Time.time;
    }

    private void OnDrawGizmos()
    {
        if (settings == null || !settings.showDebugGizmos || playerCamera == null) return;

        Vector3 pos = playerCamera.transform.position;
        Vector3 forward = playerCamera.transform.forward;

        // Draw detection sphere
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(pos, settings.maxRange);

        // Draw cone
        Gizmos.color = Color.cyan;
        Vector3 right = Quaternion.Euler(0, settings.aimConeAngle, 0) * forward;
        Vector3 left = Quaternion.Euler(0, -settings.aimConeAngle, 0) * forward;
        Gizmos.DrawRay(pos, right * settings.maxRange);
        Gizmos.DrawRay(pos, left * settings.maxRange);

        if (currentTarget != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(pos, currentTarget.position);
        }
    }
}
