using UnityEngine;

public class Teleporter : MonoBehaviour
{
    [SerializeField] Transform teleportTarget;
    [SerializeField] LayerMask separatedPlayer;
    [SerializeField] LayerMask comibinedPlayer;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("GoblinPlayer"))
        {
            if (other.gameObject.TryGetComponent<SplitGoblinController>(out SplitGoblinController controller))
            {
                controller.TeleportToPosition(teleportTarget.position);
            }
        }
        if (other.gameObject.layer == LayerMask.NameToLayer("GoblinPlayerCombined"))
        {
            // Had to make a separate one because the Combined Goblin hitbox is on another object than the script
            var controller = other.gameObject.GetComponentInParent<CombinedGoblinController>();
            if (controller != null) controller.TeleportToPosition(teleportTarget.position);

            /*
            if (other.gameObject.TryGetComponent<CombinedGoblinController>(out CombinedGoblinController controller))
            {
                controller.TeleportToPosition(teleportTarget.position);
                
            }
            */
        }
    }
}
