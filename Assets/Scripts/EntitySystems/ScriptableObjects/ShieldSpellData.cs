using UnityEngine;

/*  Owner: Ryan Brosius
 * 
 *  Scriptable Object helper that creates the shield spell when a caster calls it
 *  Factory-like pattern
 */
[CreateAssetMenu(menuName = "Spells/Defensive Shield Spell")]
public class ShieldSpellData : SpellData
{
    [Header("Shield Properties")]
    public float shieldHealth = 10f;
    public ElementalResolver elementalResolver;
    public GameObject ShieldVFX;

    public override void Cast(GameObject caster)
    {
        var bearer = caster.GetComponent<IShieldBearer>();
        if (bearer == null) return;

        bearer.ClearShield();

        var instance = Instantiate(spellPrefab, caster.transform);
        var shield = instance.GetComponent<IShield>();
        if (shield is Shield s)
            s.Initialize(shieldHealth, element, elementalResolver, ShieldVFX);

        bearer.SetShield(shield);
    }
}
