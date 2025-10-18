
/*  Owner: Ryan Brosius
 * 
 *  Interface for the affect that happens when a entity gets hit while their shield is not up
 *  Look at HitEffectDestroy for an example on how to use this
 */
public interface IHitEffectReceiver
{
    void OnHitEffect(Damage damage);
}