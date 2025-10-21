using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class Blast : MonoBehaviour, ISpell
{
    [SerializeField] private Damage damage;
    [SerializeField] private Element element;
    [SerializeField] private float duration;
    [SerializeField] private GameObject BoltVFX;
    [SerializeField] private float radius;

    public Damage Damage => damage;
    public Element Element => element;
    public float Duration => duration;

    private GameObject spellCaster;
    private Vector3 direction;
    private bool burst = false;
    private float speed;


    public void Initialize(Element elem, GameObject caster, float baseDmg, float dur, float spd, float rad, GameObject BoltVFX = null)
    {
        element = elem;
        spellCaster = caster;
        direction = caster.transform.forward;
        duration = dur;
        speed = spd;
        radius = rad;
        setDamage(baseDmg, elem, caster);
        StartCoroutine(Fizzle());
    }

      public void Update(){
        if (!burst){
            transform.position += this.transform.forward * speed * Time.deltaTime;  
        } 
    }

    public void setDamage(float baseDamage, Element elem, GameObject caster){
        var spellCaster = caster.GetComponent<ISpellCaster>();
        float damageNum = baseDamage;

        if (spellCaster.IsBuffed())
        {
            damageNum = baseDamage * spellCaster.GetDmgMultiplier();
        }

        damage = new Damage(damageNum, elem, caster);
    }

    IEnumerator Fizzle(){
        yield return new WaitForSeconds(duration);
        StartCoroutine(Burst());
    }

    void OnCollisionEnter(Collision collision){
        StopCoroutine(Fizzle());
        StartCoroutine(Burst());
        var health = collision.collider.GetComponent<HealthComponent>();
        if (health != null)
            Debug.Log("health component found");
            Debug.Log(damage.Amount);
            health.TakeDamage(damage);
    }

    private IEnumerator Burst(){
        if (!burst){
            burst = true;
            transform.localScale += new Vector3(radius, radius, radius);
            yield return new WaitForSeconds(0.5f);
            Destroy(gameObject);
        }
    }

}
