using UnityEngine;

[CreateAssetMenu( menuName = "Spells/Defensive Boon Spell")]
public class BoonSpellData : SpellData
{
    [Header("Boon Properties")]
    public GameObject BoonVFX;
    public float rotationSpeed;
    public float duration;

    public override void Cast(GameObject caster, GameObject target = null)
    {
        var instance = Instantiate(spellPrefab, caster.transform);
        var spell = instance.GetComponent<ISpell>();
        if (spell is Boon b)
                b.Initialize(element, caster, rotationSpeed, duration, BoonVFX);
    }

    public override float GetCooldown(){
        return 15f;
    }
    
}
