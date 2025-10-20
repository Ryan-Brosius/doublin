using UnityEngine;

[CreateAssetMenu(menuName = "Spells/Offensive Bolt Spell")]
public class BoltSpellData : SpellData
{
    [Header("Bolt Properties")]
    public Damage damage;

    public override void Cast(GameObject caster)
    {

        var instance = Instantiate(spellPrefab, caster.transform);
        var spell = instance.GetComponent<ISpell>();
        if (spell is Bolt b)
                b.Initialize(element, caster, null);
    }

    public float GetCooldown(){
        return 12f;
    }
}
