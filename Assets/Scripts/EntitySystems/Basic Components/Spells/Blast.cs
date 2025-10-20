using UnityEngine;
using System.Collections;

public class Blast : MonoBehaviour, ISpell
{
    [SerializeField] private Damage damage;
    [SerializeField] private Element element;
    [SerializeField] private float duration = 1f;
    [SerializeField] private GameObject BoltVFX;
    [SerializeField] private float radius;

    public Damage Damage => damage;
    public Element Element => element;
    public float Duration => duration;

    private GameObject spellCaster;
    private Vector3 direction;
    private bool burst = false;
    private float speed;


    public void Initialize(Element elem, GameObject caster, float spd, float rad, GameObject BoltVFX = null)
    {
        element = elem;
        spellCaster = caster;
        direction = spellCaster.transform.forward;
        speed = spd;
        radius = rad;
        StartCoroutine(Fizzle());
    }

    public void Update(){
        transform.position += this.transform.forward * speed * Time.deltaTime;
    }

    IEnumerator Fizzle(){
        yield return new WaitForSeconds(duration);
        StartCoroutine(Burst());
    }

    void onCollisionEnter(Collision collision){
        StopCoroutine(Fizzle());
        StartCoroutine(Burst());
        //Collision.collider.TakeDamage(damage);
    }

    private IEnumerator Burst(){
        if (!burst){
            transform.localScale += new Vector3(radius, radius, radius);
            yield return new WaitForSeconds(0.3f);
            Destroy(gameObject);
        }
        burst = true;
    }

}
