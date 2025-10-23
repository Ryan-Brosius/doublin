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

    public override void Cast(GameObject caster, Vector3 spawnPosition, GameObject target=null, Vector3? positionTarget = null)
    {
        Instantiate(spellPrefab, spawnPosition, Quaternion.identity);
    }

    public override float GetCooldown(){
        return 0;
    }
}
