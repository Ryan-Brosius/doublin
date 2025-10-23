using System;
using System.Linq;
using UnityEngine;


/*  Owner: Ryan Brosius
 * 
 *  The script to handle the shields on the goblins
 *  Maybe can get away with using this on the combined and smaller?
 *  Though in the future may need to make two seperately for that, but should be good for the combined
 */
public class GoblinShieldBearer : MonoBehaviour, IShieldBearer
{
    public IShield ActiveShield { get; private set; }
    public bool HasActiveShield => ActiveShield != null && ActiveShield.IsActive;

    public event Action<IShield> OnShieldBroken;
    public IHitEffectReceiver hitReceiver { get; set; }

    private void Awake()
    {
        hitReceiver = GetComponent<IHitEffectReceiver>();
    }

    public void SetShield(IShield newShield)
    {
        ActiveShield = newShield;
    }

    public void ClearShield()
    {
        ActiveShield = null;

        foreach (var shield in GetComponentsInChildren<MonoBehaviour>(true).OfType<IShield>())
        {
            shield.Break();
        }
    }

    public void TakeDamage(Damage damage)
    {
        if (HasActiveShield)
        {
            if (ActiveShield.AbsorbDamage(damage, out float leftover))
            {
                OnShieldBroken?.Invoke(ActiveShield);
                ActiveShield.Break();
                ActiveShield = null;
            }
            return;
        }

        hitReceiver?.OnHitEffect(damage);
    }
}
