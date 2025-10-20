using UnityEngine;

[CreateAssetMenu(menuName = "Spells/Offensive Blast Spell")]
public class BlastSpellData : SpellData
{
    [Header("Blast Properties")]
    public Damage damage;
    public float radius;

    public override void Cast(GameObject caster)
    {

        var instance = Instantiate(spellPrefab, caster.transform);
        var spell = instance.GetComponent<ISpell>();
        if (spell is Blast b)
                b.Initialize(element, caster, radius);
    }

    public float GetCooldown(){
        return 18f;
    }
}
