using UnityEngine;

[CreateAssetMenu(menuName = "Spells/Offensive Blast Spell")]
public class BlastSpellData : SpellData
{
    [Header("Blast Properties")]
    public float speed;
    public float radius;

    public override void Cast(GameObject caster)
    {

        var instance = Instantiate(spellPrefab);
        var spell = instance.GetComponent<ISpell>();
        if (spell is Blast b)
                b.Initialize(element, caster, speed, radius);
    }

    public override float GetCooldown(){
        return 18f;
    }
}
