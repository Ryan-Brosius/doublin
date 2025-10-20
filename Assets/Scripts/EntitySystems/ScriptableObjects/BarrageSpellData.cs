using UnityEngine;

[CreateAssetMenu(menuName = "Spells/Offensive Barrage Spell")]
public class BarrageSpellData : SpellData
{
    [Header("Barrage Properties")]
    public float speed;

    public override void Cast(GameObject caster)
    {

        var instance = Instantiate(spellPrefab, caster.transform.position, Quaternion.identity);
        var spell = instance.GetComponent<ISpell>();
        if (spell is Barrage b)
                b.Initialize(element, caster, speed);
    }

    public override float GetCooldown(){
        return 15f;
    }
}
