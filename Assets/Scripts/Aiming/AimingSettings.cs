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
