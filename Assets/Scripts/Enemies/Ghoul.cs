using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityHFSM;

/*
 * Owner: Cameron Romero
 *
 * Logic for Ghoul enemy
 *
 * Ghoul chases player
 * Ghoul prefab has default values for movespeed and health
 */

public class Ghoul : BaseSummon
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
