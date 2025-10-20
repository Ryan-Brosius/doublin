
/*  Owner: Ryan
 * 
 *  Interface for tracking health, useful if we want to check gameobject for health :')
 */
public interface IHealth
{
    float Current { get; }
    float Max { get; }
    void Heal(float amount);
    void SetHealth(float value);
    event System.Action<float> OnHealed;
    event System.Action<float> OnDamaged;
    event System.Action OnDied;
}