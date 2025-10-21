using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityHFSM;

/*
 * Owner: Cameron Romero
 *
 * Logic for the Skeleton enemy
 *
 * Skeleton just chases the player
 * Prefab has default values for health and movespeed
 */

public class Skeleton : BaseSummon
{
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        fsm.Init();
    }

    protected override void Update()
    {
        base.Update();
    }
}
