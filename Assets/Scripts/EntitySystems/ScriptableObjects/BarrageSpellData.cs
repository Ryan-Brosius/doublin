using UnityEngine;

[CreateAssetMenu(menuName = "Spells/Offensive Barrage Spell")]
public class BarrageSpellData : SpellData
{
    [Header("Barrage Properties")]
    public float speed;
    [Tooltip("Amount of bullets spawn from the barrage")]
    public int amount = 5;
    [Tooltip("Time between each projectile spawn")]
    public float timePerCast = .1f;

    public override void Cast(GameObject caster, Vector3 spawnPosition, GameObject target = null, Vector3? positionTarget = null)
    {

        var instance = Instantiate(spellPrefab, spawnPosition, Quaternion.identity);
        var spell = instance.GetComponent<ISpell>();
        // Do nothing for rn
        //if (spell is Barrage b)
                //b.Initialize(element, caster, speed, amount, timePerCast);
    }

    public override float GetCooldown(){
        return 15f;
    }
}
