using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityHFSM;

/* 
 * Owner: Cameron Romeroa
 *
 * Logic for the Necromancer Wizard
 */

public class Necromancer : BaseEnemy
{
    /*
     * Struct for holding the abilities for the Necromancer. 
     * Can add more abilities by adding to the enum Ability.
     * Each ability has a default cooldown and a current cooldown, only the default cooldown is set in the Inspector.
     */

    [System.Serializable]
    public class NecromancerAbilities
    {
        public enum Ability
        {
            summonSlime,
            summonSkeleton,
            summonGhouls
        }

        public Ability ability;

        [Tooltip("Cooldown for this ability")]
        public float defaultCooldown;

        [HideInInspector]
        public float currentCooldown = 0f;
    }

    // Inspector fields for the Necromancer abilities
    [SerializeField] List<NecromancerAbilities> necromancerAbilities = new List<NecromancerAbilities>();
    [SerializeField] private GameObject slimePrefab;
    [SerializeField] private GameObject skeletonPrefab;
    [SerializeField] private GameObject ghoulPrefab;
    [SerializeField] private float numberOfSlimes = 5;
    [SerializeField] private float numberOfSkeletons = 2;
    [SerializeField] private float numberOfGhouls = 1;

    // Fields for the spawn range around the player
    private float minimumSpawnRange = 3f;
    private float maximumSpawnRange = 10f;

    // Global cooldown/buffer for all spells
    [SerializeField] private float spellBuffer = 5f;
    private float spellTimer = 0f;

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

    // Will cycle through all abilities and reduce their cooldowns
    void Update()
    {
        base.Update();
        fsm.OnLogic();

        for (int i = 0; i < necromancerAbilities.Count; i++)
        {
            if (necromancerAbilities[i].currentCooldown > 0f)
            {
                necromancerAbilities[i].currentCooldown -= Time.deltaTime;
            }
        }
        if (spellTimer > 0f)
            spellTimer -= Time.deltaTime;
    }

    /*
     * Attack State
     *
     * When adding more abilities, add more cases in the switch statement and link the ability name and the function for that ability.
     * Will reset ability cooldown after cast.
     */

    private void Attack()
    {
        for (int i = 0; i < necromancerAbilities.Count; i++)
        {
            var ability = necromancerAbilities[i];

            if (ability.currentCooldown <= 0f && spellTimer <= 0f)
            {
                switch (ability.ability)
                {
                    case NecromancerAbilities.Ability.summonSlime:
                        SummonSlimes();
                        break;
                    case NecromancerAbilities.Ability.summonSkeleton:
                        SummonSkeletons();
                        break;
                    case NecromancerAbilities.Ability.summonGhouls:
                        SummonGhouls();
                        break;
                }
                spellTimer = 2f;
                ability.currentCooldown = ability.defaultCooldown;
                break;
            }
        }
    }

    // Calculates a point in a radius around the player within the min and max distances set above.
    private Vector3 calculateSummonPoint()
    {
        float angle = Random.Range(0f, Mathf.PI * 2f);

        float distance = Random.Range(minimumSpawnRange, maximumSpawnRange);
        float xOffset = Mathf.Cos(angle) * distance;
        float zOffset = Mathf.Sin(angle) * distance;

        Vector3 spawnPoint = playerTransform.position + new Vector3(xOffset, 0, zOffset);

        return spawnPoint;
    }

    // For the summon spells, each individual summoned entity has their own unique summon point.
    // To have summoned groups spawn at the same point, like the slime or skeleton, move calculateSummonPoint() outside the loop and set a small offset in the loop.
    private void SummonSlimes()
    {
        for (int i = 0; i < numberOfSlimes; i++)
        {
            Instantiate(slimePrefab, calculateSummonPoint(), Quaternion.identity);
        }
    }

    private void SummonSkeletons()
    {
        for (int i = 0; i < numberOfSkeletons; i++)
        {
            Instantiate(skeletonPrefab, calculateSummonPoint(), Quaternion.identity);
        }
    }

    private void SummonGhouls()
    {
        for (int i = 0; i < numberOfGhouls; i++)
        {
            Instantiate(ghoulPrefab, calculateSummonPoint(), Quaternion.identity);
        }
    }
}
