using UnityEngine;

/*  Owner: Ryan Brosius
 * 
 *  Script that points/rotates an object towards where the AimingManager is focusing
 *  Currently used for pointing the wand towards entities, but can be used for anthing else if needed
 */
public class RotateTowardsTarget : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float rotationSpeed = 360f;
    [Tooltip("Determines if rotation will only happen when looking at an entity")]
    [SerializeField] bool onlyPointTowardsTargets = true;

    private Quaternion startingLocalRotation;

    private LazyDependency<AimingManager> _AimingManager;

    private void Awake()
    {
        if (onlyPointTowardsTargets)
            startingLocalRotation = transform.localRotation;
    }

    private void Update()
    {
        Quaternion targetRotation;

        if (_AimingManager.Value.CurrentTarget != null)
        {
            Vector3 direction = _AimingManager.Value.CurrentTarget.position - transform.position;
            targetRotation = Quaternion.LookRotation(direction);
        }
        else if (!onlyPointTowardsTargets)
        {
            Vector3 direction = _AimingManager.Value.CrosshairPosition - transform.position;
            targetRotation = Quaternion.LookRotation(direction);
        }
        else
        {
            targetRotation = transform.parent.rotation * startingLocalRotation;
        }

        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }
}
