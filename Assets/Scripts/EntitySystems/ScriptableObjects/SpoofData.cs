using UnityEngine;

public enum SpoofType { Invalid, Cancel, Cooldown }

/*  Owner: Mackenzie Ligon
 *  
 * Visual effect to denote the player has failed to cast a spell
 * Different colors will indicate why the spell failed
 */

[CreateAssetMenu(menuName = "Spells/Spoof Data")]
public class SpoofData : SpellData
{
    [Header("Spoof Info")]
    public SpoofType spoofType;

    public override void Cast(GameObject caster, GameObject target=null)
    {
        Instantiate(spellPrefab, caster.transform);
    }

    public override float GetCooldown(){
        return 0;
    }
}
