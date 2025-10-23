using UnityEngine;

[CreateAssetMenu(menuName = "Spells/Offensive Bolt Spell")]
public class BoltSpellData : SpellData
{
    [Header("Bolt Properties")]
    public float speed;
    public float duration;

    public override void Cast(GameObject caster, Vector3 spawnPosition, GameObject target=null, Vector3? positionTarget = null)
    {
        var instance = Instantiate(spellPrefab, spawnPosition, spellPrefab.transform.rotation);
        var spell = instance.GetComponent<ISpell>();
        if (spell is Bolt b)
        {
            if (target != null)
                b.Initialize(element, caster, target.transform.position, baseDamage, duration, speed);
            else if (positionTarget != null)
                b.Initialize(element, caster, positionTarget.Value, baseDamage, duration, speed);
            else
                b.Initialize(element, caster, caster.transform.position, baseDamage, duration, speed);
        }
    }

    public override float GetCooldown(){
        return cooldown;
    }
}
