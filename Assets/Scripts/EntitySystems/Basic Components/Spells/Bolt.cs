using UnityEngine;
using System.Collections;


[RequireComponent(typeof(Collider))]
public class Bolt : OffensiveSpell
{
    [SerializeField] private GameObject BoltVFX;

    private float speed;

    public void Initialize(Element elem, GameObject caster, Vector3 target, float baseDmg, float dur, float spd, GameObject BoltVFX = null)
    {
        base.Initialize(elem, caster, target, baseDmg, dur);
        speed = spd;
        StartCoroutine(Fizzle());
    }

    public void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }
}
