using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityHFSM;

/* Owner: Cameron Romero
 * 
 * Logic for the Necromancer Wizard
 */

public class Necromancer : BaseEnemy
{
    [System.Serializable]
    public class NecromancerAbilities
    {
        public enum Ability
        {
            summonSlime
        }

        public Ability ability;
        public float cooldown;
    }

    private float lastAttackTime = -Mathf.Infinity;
    [SerializeField] List<NecromancerAbilities> necromanceAbilities;
    private int attackIndex = 0;

    [SerializeField] private GameObject slimePrefab;
    [SerializeField] private GameObject skeletonPrefab;
    [SerializeField] private GameObject boneSpikePrefab;

    [SerializeField] private float numberOfSlimes = 5;

    [SerializeField] private Transform summonPoint;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();

        fsm.AddState("Idle", new State(
              onLogic: state => {/* do nothing */}
        ));

        fsm.AddState("Attack", new State(
              onLogic: state => Attack()
        ));

        fsm.SetStartState("Idle");

        fsm.AddTwoWayTransition("Idle", "Attack",
            transition => playerTransform != null && distanceToPlayer < scanningRange);

        fsm.Init();
    }

    void Update()
    {
        base.Update();
        fsm.OnLogic();
    }

    private void Attack()
    {


        SummonSlimes();

        attackIndex = 0;
        lastAttackTime = Time.time;
    }

    private void SummonSlimes()
    {
        for (int i = 0; i < numberOfSlimes; i++)
        {
            Vector3 spawnOffset = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));

            Instantiate(slimePrefab, summonPoint.position + spawnOffset, Quaternion.identity);
        }
    }
}
