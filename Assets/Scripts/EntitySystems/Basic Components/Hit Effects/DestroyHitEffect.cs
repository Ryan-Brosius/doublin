using UnityEngine;

/*  Owner: Ryan Brosius
 * 
 *  Simple hit effect that destroys the object whenever it gets hit
 */
public class DestroyHitEffect : MonoBehaviour, IHitEffectReceiver
{
    public void OnHitEffect(Damage damage)
    {
        Destroy(gameObject);
    }
}
