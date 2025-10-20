using UnityEngine;

[CreateAssetMenu(menuName = "Spells/Offensive Bolt Spell")]
public class BoltSpellData : SpellData
{
    [Header("Bolt Properties")]
    public float speed;

    public override void Cast(GameObject caster)
    {

        var instance = Instantiate(spellPrefab);
        var spell = instance.GetComponent<ISpell>();
        if (spell is Bolt b)
                b.Initialize(element, caster, speed);
    }

    public override float GetCooldown(){
        return 12f;
    }
}
