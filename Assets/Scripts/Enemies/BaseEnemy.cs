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
 */

[RequireComponent(typeof(Rigidbody))]
public class BaseEnemy : MonoBehaviour
{
    protected Transform target;
    protected float distanceToPlayer;
    protected Rigidbody rb;
    protected GameObject[] players;

    [SerializeField] protected float groundCheckDistance = 0.4f;
    [SerializeField] protected Vector3 groundCheckOffset = Vector3.zero;

    [SerializeField] protected float scanningRange = 30f;
    [SerializeField] protected float maxHealth = 100f;

    protected StateMachine fsm;

    [SerializeField] protected LayerMask groundLayerMask;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    protected virtual void Start()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        fsm = new StateMachine();
    }

    protected virtual void Update()
    {
        distanceToPlayer = DistanceToTarget;
    }

    public virtual void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    protected virtual void FindRandomTarget()
    {
        if (players.Length > 0)
        {
            int randomIndex = Random.Range(0, players.Length);
            target = players[randomIndex].transform;
        }
    }

    protected virtual void FindClosestTarget()
    {
        if (players.Length > 0)
        {
            Transform closest = null;
            float minDistance = float.MaxValue;

            foreach (GameObject player in players)
            {
                float distance = Vector3.Distance(transform.position, player.transform.position);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    closest = player.transform;
                }
            }
            target = closest;
        }

    }

    protected float DistanceToTarget
    {
        get
        {
            if (target == null) return float.MaxValue;
            return Vector3.Distance(transform.position, target.position);
        }
    }

    protected bool IsGrounded()
    {
        Vector3 origin = transform.position + groundCheckOffset;
        return Physics.Raycast(origin, Vector3.down, groundCheckDistance);
    }

    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 origin = transform.position + groundCheckOffset;
        Vector3 end = origin + Vector3.down * groundCheckDistance;
        Gizmos.DrawLine(origin, end);
        Gizmos.DrawSphere(end, 0.05f);

    }
}
