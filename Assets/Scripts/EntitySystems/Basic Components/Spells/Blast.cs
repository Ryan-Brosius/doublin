using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class Blast : OffensiveSpell
{
    [SerializeField] private GameObject BlastVFX;
    [SerializeField] private float radius;

    private bool burst = false;
    private float speed;


    public void Initialize(Element elem, GameObject caster, GameObject target, float baseDmg, float dur, float spd, float rad, GameObject BoltVFX = null)
    {
        base.Initialize(elem, caster, target, baseDmg, dur);
        speed = spd;
        radius = rad;
        StartCoroutine(Fizzle());
    }

      public void Update(){
        if (!burst){
            transform.position += direction * speed * Time.deltaTime;  
        } 
    }


    protected override void OnCollisionEnter(Collision collision){
        StopCoroutine(Fizzle());
        StartCoroutine(Burst());
        var health = collision.collider.GetComponent<HealthComponent>();
        if (health != null)
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
