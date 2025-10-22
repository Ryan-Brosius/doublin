using UnityEngine;
using System.Collections;


[RequireComponent(typeof(Collider))]
public class Bolt : MonoBehaviour, ISpell
{
    [SerializeField] private Damage damage;
    [SerializeField] private Element element;
    [SerializeField] private float duration;
    [SerializeField] private GameObject BoltVFX;

    public Damage Damage => damage;
    public Element Element => element;
    public float Duration => duration;

    private GameObject spellCaster;
    private Vector3 direction;
    private float speed;


    public void Initialize(Element elem, GameObject caster, GameObject target, float baseDmg, float dur, float spd, GameObject BoltVFX = null)
    {
        element = elem;
        spellCaster = caster;
        direction = caster.transform.forward;
        duration = dur;
        speed = spd;
        SetDirection(target);
        SetDamage(baseDmg, elem, caster);
        StartCoroutine(Fizzle());
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

    public void SetDamage(float baseDamage, Element elem, GameObject caster){
        var spellCaster = caster.GetComponent<ISpellCaster>();
        float damageNum = baseDamage;

        if (spellCaster.IsBuffed())
        {
            damageNum = baseDamage *  spellCaster.GetDmgMultiplier();
        }

        damage = new Damage(damageNum, elem, caster);
    }

    public void Update(){
        transform.position += direction * speed * Time.deltaTime;
    }

    IEnumerator Fizzle(){
        yield return new WaitForSeconds(duration);
        Destroy(gameObject);
    }

    void OnCollisionEnter(Collision collision){
        StopCoroutine(Fizzle());
        var health = collision.collider.GetComponent<HealthComponent>();
        if (health != null)
            health.TakeDamage(damage);
        Destroy(gameObject); 
    }

   
}
