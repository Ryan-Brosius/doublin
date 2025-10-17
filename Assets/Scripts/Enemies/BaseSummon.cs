using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityHFSM;

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
    }

    protected void Chase()
    {
        if (playerTransform == null) return;

        Vector3 direction = (playerTransform.position - transform.position).normalized;
        rb.linearVelocity = new Vector3(direction.x * moveSpeed, rb.linearVelocity.y, direction.z * moveSpeed);
    }
}
