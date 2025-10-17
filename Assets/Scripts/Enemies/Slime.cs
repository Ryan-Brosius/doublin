using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityHFSM;

public class Slime : BaseSummon
{
    [SerializeField] private float horizontalJumpForce = 2f;
    [SerializeField] private float verticalJumpForce = 4f;
    [SerializeField] private float jumpRange = 3f;

    protected override void Awake()
    {
        base.Awake();
    }

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

    protected override void Update()
    {
        base.Update();
        fsm.OnLogic();
    }

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
