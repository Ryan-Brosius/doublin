using System.Collections.Generic;
using UnityEngine;

/*  Owner: Ryan Brosius
 *  
 *  Drag-and-drop script to make an entity a shield bearer
 *  The wizard will continue to cast the shield phases that they have until they run out of shields to cast
 */
public class WizardShieldBearer : MonoBehaviour, IShieldBearer
{
    [SerializeField]
    [Tooltip("The prefab shields that the wizard will spawn on itself")]
    private List<Shield> shieldPhases = new List<Shield>();
    private int currentIndex = 0;
    public IShield ActiveShield { get; private set; }
    public bool HasActiveShield => ActiveShield != null && ActiveShield.IsActive;
    public bool InExecuteRange { get; private set; }

    public event System.Action<IShield> OnShieldBroken;
    public IHitEffectReceiver hitReceiver { get; set; }

    private void Awake()
    {
        hitReceiver = GetComponent<IHitEffectReceiver>();
    }

    private void Start()
    {
        ActivateNextShield();
    }

    private void ActivateNextShield()
    {
        // Check if we have any shields left to instantiate
        if (currentIndex < shieldPhases.Count)
        {
            ActiveShield = Instantiate(shieldPhases[currentIndex], transform);
            currentIndex++;
        }
        else
        {
            // No more casting :)
            InExecuteRange = true;
            ActiveShield = null;
        }
    }

    public void TakeDamage(Damage damage)
    {
        // Check if we have active shield, if not then likely we can set off our on-hit effect
        if (HasActiveShield)
        {
            // If shield is fully absorbed we can try to spawn our next shield
            if (ActiveShield.AbsorbDamage(damage, out float _))
            {
                OnShieldBroken?.Invoke(ActiveShield);
                ActiveShield.Break();
                ActiveShield = null;
                ActivateNextShield();
            }
        }
        else if (InExecuteRange)
        {
            // Play our hit effect
            hitReceiver?.OnHitEffect(damage);
        }
    }

    public void SetShield(IShield newShield) => ActiveShield = newShield;
    public void ClearShield() => ActiveShield = null;
}
