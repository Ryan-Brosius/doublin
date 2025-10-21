using System;
using UnityEngine;

/*  Owner: Ryan Brosius
 * 
 *  Drag-and-drop health component that can be put onto entities
 */
[RequireComponent(typeof(Collider))]
public class HealthComponent : MonoBehaviour, IHealth, IDamageable
{
    [SerializeField] private float maxHealth = 1.0f;
    public float Current {  get; private set; }
    public float Max => maxHealth;

    public event Action<float> OnHealed;
    public event Action<float> OnDamaged;
    public event Action OnDied;

    private void Awake()
    {
        Current = maxHealth;
    }

    public void Heal(float amount)
    {
        Current = Mathf.Clamp(Current + amount, 0, maxHealth);
        OnHealed?.Invoke(Current);
    }

    public void SetHealth(float value)
    {
        Current = Mathf.Clamp(value, 0, maxHealth);
        if (Current <= 0) Die();
    }

    public void TakeDamage(Damage damage)
    {
        Current -= damage.Amount;

        if (Current <= 0) Die();
        else OnDamaged?.Invoke(Current);
    }

    private void Die()
    {
        OnDied?.Invoke();
        Destroy(gameObject);
    }
}
