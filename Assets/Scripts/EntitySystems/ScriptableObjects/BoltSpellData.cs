using UnityEngine;

[CreateAssetMenu(menuName = "Spells/Offensive Bolt Spell")]
public class BoltSpellData : SpellData
{
    [Header("Bolt Properties")]
    public float speed;
    public float duration;

    public override void Cast(GameObject caster)
    {
        Vector3 spawnSpot = caster.transform.position + caster.transform.forward;
        var instance = Instantiate(spellPrefab, spawnSpot, spellPrefab.transform.rotation);
        var spell = instance.GetComponent<ISpell>();
        if (spell is Bolt b)
                b.Initialize(element, caster, baseDamage, duration, speed);
    }

    public override float GetCooldown(){
        return 12f;
    }
}
