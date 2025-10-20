using UnityEngine;
using System.Collections;

public class Bolt : MonoBehaviour, ISpell
{
    [SerializeField] private Damage damage;
    [SerializeField] private Element element;
    [SerializeField] private float duration = 3f;
    [SerializeField] private GameObject BoltVFX;

    public Damage Damage => damage;
    public Element Element => element;
    public float Duration => duration;

    private GameObject spellCaster;
    private Vector3 direction;


    public void Initialize(Element elem, GameObject caster, GameObject BoltVFX = null)
    {
        element = elem;
        spellCaster = caster;
        direction = spellCaster.transform.forward;
        StartCoroutine(Fizzle());
    }

    public void Update(){
        transform.position += this.transform.forward * 0.1f;
    }

    IEnumerator Fizzle(){
        yield return new WaitForSeconds(duration);
        Destroy(gameObject);
    }

    void onCollisionEnter(Collision collision){
        StopCoroutine(Fizzle());
        Destroy(gameObject);
        //Collision.collider.TakeDamage(damage);
    }

   
}
