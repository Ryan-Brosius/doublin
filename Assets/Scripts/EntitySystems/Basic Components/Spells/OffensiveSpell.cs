using UnityEngine;
using System.Collections;

public abstract class OffensiveSpell : MonoBehaviour, ISpell
{
    [SerializeField] protected Damage damage;
    [SerializeField] protected Element element;
    [SerializeField] protected float duration;

    protected GameObject spellCaster;
    protected Vector3 direction;

    public Element Element => element;
    public float Duration => duration;

    public void Initialize(Element elem, GameObject caster, Vector3 target, float baseDmg, float dur)
    {
        element = elem;
        spellCaster = caster;
        duration = dur;
        SetDirection(target);
        SetDamage(baseDmg, elem, caster);
    }

    private void SetDirection(Vector3 target)
    {
        transform.LookAt(target);
        direction = transform.forward;
    }

    public void SetDamage(float baseDamage, Element elem, GameObject caster)
    {
        var spellCaster = caster.GetComponent<ISpellCaster>();
        float damageNum = baseDamage;

        // Commenting this out below because Im refactoring code how to attack entities
        // This buffing should be re-worked in the future if this gets picked up again

        //if (spellCaster.IsBuffed())
        //{
        //    damageNum = baseDamage *  spellCaster.GetDmgMultiplier();
        //}

        damage = new Damage(damageNum, elem, caster);
    }

    protected virtual IEnumerator Fizzle()
    {
        yield return new WaitForSeconds(duration);
        Destroy(gameObject);
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        var damageable = collision.collider.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(damage);
            Destroy(gameObject);
        }
    }

}
