using UnityEngine;

public class PlayerSeperateHitEffect : MonoBehaviour, IHitEffectReceiver
{
    private LazyDependency<GoblinStateManager> _GoblinStateManager;
    [SerializeField] private float force = 10f;

    public void OnHitEffect(Damage damage)
    {
        if (gameObject.TryGetComponent<CombinedGoblinController>(out CombinedGoblinController controller))
        {
            controller.HandleSeperateNoThrow();

            // Random direction
            Vector3 horizontal = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;
            float angle = Random.Range(0f, 90f);
            Quaternion tilt = Quaternion.AngleAxis(angle, Vector3.Cross(Vector3.up, horizontal));
            Vector3 forceDir = tilt * Vector3.up;
            _GoblinStateManager.Value.GrimoireGoblinController.ApplyForce(forceDir, force, ForceMode.VelocityChange);

            horizontal = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;
            angle = Random.Range(0f, 90f);
            tilt = Quaternion.AngleAxis(angle, Vector3.Cross(Vector3.up, horizontal));
            forceDir = tilt * Vector3.up;
            _GoblinStateManager.Value.StaffGoblinController.ApplyForce(forceDir, force, ForceMode.VelocityChange);
        }
    }
}
