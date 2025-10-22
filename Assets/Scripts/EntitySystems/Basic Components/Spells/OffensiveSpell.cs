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

    public void Initialize(Element elem, GameObject caster, GameObject target, float baseDmg, float dur)
    {
        element = elem;
        spellCaster = caster;
        duration = dur;
        SetDirection(target);
        SetDamage(baseDmg, elem, caster);
    }

    private void SetDirection(GameObject target)
    {
        if (target){
            direction = Vector3.Normalize(target.transform.position - transform.position);
        }
        else 
        {
            direction = spellCaster.transform.forward;
        }
    }

    public void SetDamage(float baseDamage, Element elem, GameObject caster)
    {
        var spellCaster = caster.GetComponent<ISpellCaster>();
        float damageNum = baseDamage;

        if (spellCaster.IsBuffed())
        {
            damageNum = baseDamage *  spellCaster.GetDmgMultiplier();
        }

        damage = new Damage(damageNum, elem, caster);
    }

    protected virtual IEnumerator Fizzle()
    {
        yield return new WaitForSeconds(duration);
        Destroy(gameObject);
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        StopCoroutine(Fizzle());
        var health = collision.collider.GetComponent<HealthComponent>();
        if (health != null)
            health.TakeDamage(damage);
        Destroy(gameObject); 
    }

}
