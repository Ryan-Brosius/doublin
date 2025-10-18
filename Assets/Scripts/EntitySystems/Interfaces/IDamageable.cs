
/*  Owner: Ryan
 *  
 *  Interface to give objects damageable property
 *  Very useful when checking if we can call damageable on an entity
 */
public interface IDamageable
{
    void TakeDamage(Damage damage);
}