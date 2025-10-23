using UnityEngine;

// Spell Type to reference type of spell in the future
public enum SpellType { Offensive, Defensive, Spoof }


/*  Owner: Ryan Brosius
 *  
 *  Spell data scriptable object to contain information about creation of a spell
 *  Contains basic information all spells should derive from- but may not use... and thats okay
 *  Cast is for setting up the creation of the spell itself, similar to a factory, look at ShieldSpellData for an example
 */
[CreateAssetMenu(menuName = "Spells/Base Spell Data")]
public abstract class SpellData : ScriptableObject
{
    [Header("General Info")]
    public string spellName;
    public SpellType spellType;
    public Element element;
    public float baseDamage = 0f;
    public float cooldown = 3f;

    [Header("Prefab")]
    public GameObject spellPrefab;

    // Should derive from this to instantiate the spell itself
    // If default target is null, default to positionTarget, if that is null default to whatever makes the most sense
    public abstract void Cast(GameObject caster, Vector3 spawnPosition, GameObject target=null, Vector3? positionTarget=null);

    public virtual float GetCooldown(){
        return cooldown;
    }
}