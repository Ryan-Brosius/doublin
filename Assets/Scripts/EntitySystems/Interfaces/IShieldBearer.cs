
/*  Owner: Ryan Brosius
 * 
 *  Interface for a ShieldBearer
 *  Contains the functions that a entity that can use shields should contain
 *  Look at WizardShieldBearer for an example on how to use
 */
public interface IShieldBearer : IDamageable
{
    bool HasActiveShield { get; }
    IShield ActiveShield { get; }
    IHitEffectReceiver hitReceiver { get; set; }
    event System.Action<IShield> OnShieldBroken;
    void SetShield(IShield shield);
    void ClearShield();
}
