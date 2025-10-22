using UnityEngine;
using System.Collections;

public class Boon : MonoBehaviour, ISpell
{
    [SerializeField] private Element element;
    [SerializeField] private float duration;
    [SerializeField] private GameObject BoonVFX;
    [SerializeField] private float speed;
    [SerializeField, Range(1f, 3f)] private float boonScaleMultiplier = 1.7f;

    public Element Element => element;
    public float Duration => duration;

    private GameObject spellCaster;

    private void Awake()
    {
        ScaleBoonVFX();
    }

    public void Initialize(Element elem, GameObject caster, float spd, float dur, GameObject BoonVFX = null)
    {
        element = elem;
        spellCaster = caster;
        speed = spd;
        duration = dur;
        BuffCaster();
        StartCoroutine(Fizzle());
    }

    void Update()
    {   
        transform.Rotate(Vector3.up, speed * Time.deltaTime);
    }

    IEnumerator Fizzle(){
        yield return new WaitForSeconds(duration);
        ClearCasterBuff();
        Destroy(gameObject);
    }

    public void BuffCaster()
    {
        //eventually will reference the spell database to figure out where to apply the buff 
        var casterInstance = spellCaster.GetComponent<ISpellCaster>(); //for cast buffs 
        //another line would get the goblin controllers for the speed buff 
        switch (element)
        {
            case (Element.Fire):
                if (casterInstance is PlayerSpellCaster c)
                    c.SetDmgBuff(true);
                break;
            default:
                break;
        }
    }

    public void ClearCasterBuff()
    {
        //eventually will reference the spell database to figure out where to apply the buff 
        var casterInstance = spellCaster.GetComponent<ISpellCaster>(); //for cast buffs 
        //another line would get the goblin controllers for the speed buff 
        switch (element)
        {
            case (Element.Fire):
                if (casterInstance is PlayerSpellCaster c)
                    c.SetDmgBuff(false);
                break;
            default:
                break;
        }
    }

    private void ScaleBoonVFX()
    {
        if (BoonVFX == null)
            return;

        // Grab the parent that we are attatched to
        Transform parent = transform.parent;
        if (parent == null)
            // If parent not found just bail, was not spawned/attatched properly
            return;

        BoonVFX.transform.SetParent(parent);

        // Grab all renderers in the parent
        Renderer[] renderers = parent.GetComponentsInChildren<Renderer>();

        // If no renderers are found then just bail
        if (renderers.Length == 0)
            return;

        // Grow a bounds to the size of the largest from the center
        Bounds totalBounds = renderers[0].bounds;
        foreach (var renderer in renderers)
            totalBounds.Encapsulate(renderer.bounds);

        // Scale up the sphere VFX as needed to be largest bounds size * multiplier
        float maxSize = Mathf.Max(totalBounds.size.x, totalBounds.size.y, totalBounds.size.z);
        BoonVFX.transform.localScale = BoonVFX.transform.localScale * maxSize * boonScaleMultiplier;
    }
}
