using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityHFSM;

/*
 * Owner: Cameron Romero
 *
 * Base class for all enemy types
 * Contains common functionality and properties for all enemies
 *
 * TakeDamage() and Die() are not being used as of 10/17, will change when needed
 */

public class BaseEnemy : MonoBehaviour
{
    protected Transform playerTransform;
    protected float distanceToPlayer;
    protected Rigidbody rb;

    [SerializeField] protected float scanningRange = 30f;
    [SerializeField] protected float maxHealth = 100f;
    protected float currentHealth;

    protected StateMachine fsm;

    protected virtual void Awake()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody>();
    }

    protected virtual void Start()
    {
        playerTransform = GameObject.FindWithTag("Player").transform;
        fsm = new StateMachine();
    }

    protected virtual void Update()
    {
        distanceToPlayer = Vector3.Distance(playerTransform.position, transform.position);
    }

    public virtual void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }
}
