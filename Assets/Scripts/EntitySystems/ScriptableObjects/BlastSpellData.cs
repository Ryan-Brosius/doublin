using UnityEngine;

[CreateAssetMenu(menuName = "Spells/Offensive Blast Spell")]
public class BlastSpellData : SpellData
{
    [Header("Blast Properties")]
    public float speed;
    public float duration;
    public float radius;

    public override void Cast(GameObject caster, GameObject target=null)
    {
        Vector3 spawnSpot = caster.transform.position + caster.transform.forward;
        var instance = Instantiate(spellPrefab, spawnSpot, spellPrefab.transform.rotation);
        var spell = instance.GetComponent<ISpell>();
        if (spell is Blast b)
                b.Initialize(element, caster, target, baseDamage, duration, speed, radius);
    }

    public override float GetCooldown(){
        return 18f;
    }

}
