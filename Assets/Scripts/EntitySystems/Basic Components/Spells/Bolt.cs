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


    public void Initialize(Element elem, GameObject caster, float baseDmg, float dur, float spd, GameObject BoltVFX = null)
    {
        element = elem;
        spellCaster = caster;
        direction = caster.transform.forward;
        duration = dur;
        speed = spd;
        setDamage(baseDmg, elem, caster);
        StartCoroutine(Fizzle());
    }

    public void setDamage(float baseDamage, Element elem, GameObject caster){
        var spellCaster = caster.GetComponent<ISpellCaster>();
        float damageNum = baseDamage;

        if (spellCaster.IsBuffed())
        {
            damageNum = baseDamage *  spellCaster.GetDmgMultiplier();
        }

        damage = new Damage(damageNum, elem, caster);
    }

    public void Update(){
        transform.position += this.transform.forward * speed * Time.deltaTime;
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
