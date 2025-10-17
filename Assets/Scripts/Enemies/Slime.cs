using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityHFSM;

/*
 * Owner: Cameron Romero
 *
 * Logic for Slime enemy
 */

public class Slime : BaseSummon
{
    // Iterable fields for the slime jump, default values also set in Slime prefab
    [SerializeField] private float horizontalJumpForce = 2f;
    [SerializeField] private float verticalJumpForce = 4f;
    [SerializeField] private float jumpRange = 3f;

    protected override void Awake()
    {
        base.Awake();
    }

    // If the slime is within the jump range and is grounded, the slime will jump at the player
    protected override void Start()
    {
        base.Start();

        fsm.AddState("Jump", new State(
              onEnter: state => Jump(),
              onLogic: state => Chase()
              ));

        fsm.AddTwoWayTransition("Chase", "Jump",
            transition => playerTransform != null && distanceToPlayer <= jumpRange && IsGrounded());

        fsm.Init();
    }

    protected void Update()
    {
        base.Update();
    }

    // Basic jump function, just changes linear velocity of the rb to the jump vector
    private void Jump()
    {
        if (playerTransform == null || !IsGrounded()) return;

        Vector3 jumpDirection = (playerTransform.position - transform.position).normalized;
        jumpDirection.y = 0;

        Vector3 jumpVector = (jumpDirection * horizontalJumpForce) + (Vector3.up * verticalJumpForce);

        rb.linearVelocity = jumpVector;
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 0.4f);
    }
}
