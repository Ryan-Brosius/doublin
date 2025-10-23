using UnityEngine;

[CreateAssetMenu(menuName = "Spells/Offensive Blast Spell")]
public class BlastSpellData : SpellData
{
    [Header("Blast Properties")]
    public float speed;
    public float duration;
    public float radius;

    public override void Cast(GameObject caster, Vector3 spawnPosition, GameObject target=null, Vector3? positionTarget = null)
    {
        Vector3 spawnSpot = spawnPosition;
        var instance = Instantiate(spellPrefab, spawnSpot, spellPrefab.transform.rotation);
        var spell = instance.GetComponent<ISpell>();
        if (spell is Blast b)
        {
            if (target != null)
                b.Initialize(element, caster, target.transform.position, baseDamage, duration, speed, radius);
            else if (positionTarget != null)
                b.Initialize(element, caster, positionTarget.Value, baseDamage, duration, speed, radius);
            else
                b.Initialize(element, caster, caster.transform.position, baseDamage, duration, speed, radius);
        }
    }

    public override float GetCooldown(){
        return cooldown;
    }
}
