using UnityEngine;
using System.Collections;
using UnityEngine.UIElements;

public class Barrage :  MonoBehaviour, ISpell
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
    private float speed;


    public void Initialize(Element elem, GameObject caster, Vector3 target, float spd, GameObject BoltVFX = null)
    {
        SetDirection(target);
        element = elem;
        spellCaster = caster;
        direction = spellCaster.transform.forward;
        speed = spd;
        StartCoroutine(Fizzle());
    }

    public void Update(){
        transform.position += this.transform.forward * speed * Time.deltaTime;
    }

    IEnumerator Fizzle(){
        yield return new WaitForSeconds(duration);
        Destroy(gameObject);
    }

    private void SetDirection(Vector3 target)
    {
        transform.LookAt(target);
        direction = transform.forward;
    }
}
