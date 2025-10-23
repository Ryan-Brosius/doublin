using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
public class Blast : OffensiveSpell
{
    [SerializeField] private GameObject BlastVFX;
    [SerializeField] private float radius;
    private float explosionForce = 50f;
    private float upwardModifier = 1f;

    private bool burst = false;
    private float speed;
    private Collider blastCollider;

    private void Awake()
    {
        blastCollider = GetComponent<Collider>();
    }

    public void Initialize(Element elem, GameObject caster, Vector3 target, float baseDmg, float dur, float spd, float rad, GameObject BoltVFX = null)
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

        var damageable = collision.collider.GetComponent<IDamageable>();
        if (damageable != null)
            damageable.TakeDamage(damage);

        StartCoroutine(Burst());
    }

    private IEnumerator Burst(){
        if (burst) yield break;
        burst = true;

        if (blastCollider != null)
            blastCollider.enabled = false;

        if (BlastVFX != null)
            Instantiate(BlastVFX, transform.position, Quaternion.identity);

        transform.localScale += new Vector3(radius, radius, radius);

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);
        HashSet<GameObject> affectedObjects = new HashSet<GameObject>();

        foreach (var hit in hitColliders)
        {
            if (affectedObjects.Contains(hit.gameObject))
                continue;
            affectedObjects.Add(hit.gameObject);

            var dmg = hit.GetComponent<IDamageable>();
            if (dmg != null && dmg != spellCaster.GetComponent<IDamageable>())
                dmg.TakeDamage(damage);

            var rb = hit.attachedRigidbody;
            if (rb != null && rb != spellCaster.GetComponent<Rigidbody>())
                ApplyBlastForce(rb);
        }

        yield return new WaitForSeconds(0.2f);
        Destroy(gameObject);
    }

    private void ApplyBlastForce(Rigidbody rb)
    {
        Vector3 direction = (rb.worldCenterOfMass - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, rb.worldCenterOfMass);
        float falloff = Mathf.Clamp01(1f - (distance / radius));

        rb.AddForce(direction * explosionForce * falloff + Vector3.up * upwardModifier, ForceMode.Impulse);
    }
}
