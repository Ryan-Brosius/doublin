using UnityEngine;

public class ContactDamage : MonoBehaviour
{
    [SerializeField] float contactDamage = 1f;
    [SerializeField] Element contactElementType = Element.None;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            var damage = collision.gameObject.GetComponent<IDamageable>();
            if (damage == null)
                return;

            damage.TakeDamage(
                new Damage(
                    contactDamage,
                    contactElementType,
                    gameObject
                )
            );
        }
    }
}
