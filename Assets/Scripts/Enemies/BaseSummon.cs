using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityHFSM;

/*
 * Owner: Cameron Romero
 *
 * Base class for all summons
 *
 * Contains field moveSpeed
 * Sets default state to "Chase"
 * Defines Chase() which moves the summon in the direction of the player at its set movespeed
 */

public class BaseSummon : BaseEnemy
{
    [SerializeField] protected float moveSpeed = 3f;
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();

        fsm.AddState("Chase", new State(
              onLogic: state => Chase()
              ));

        fsm.SetStartState("Chase");
    }

    protected override void Update()
    {
        base.Update();
        fsm.OnLogic();
        FindClosestTarget();
        Debug.Log(target);
    }

    protected void Chase()
    {
        if (target == null) return;

        Vector3 direction = (target.position - transform.position).normalized;
        rb.linearVelocity = new Vector3(direction.x * moveSpeed, rb.linearVelocity.y, direction.z * moveSpeed);
    }
}
